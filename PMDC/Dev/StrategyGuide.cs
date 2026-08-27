using DynamicData;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PMDC.Data;
using PMDC.Dungeon;
using PMDC.LevelGen;
using RogueElements;
using RogueEssence;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.LevelGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace PMDC.Dev
{
    public static class StrategyGuide
    {
        private const int TOTAL_CHUNKS = 60;


        public static void DeleteWiki()
        {
            if (Directory.Exists(PathMod.APP_PATH + "WIKI/"))
                Directory.Delete(PathMod.APP_PATH + "WIKI/", true);
        }

        private static bool WriteToWiki(string name, string content)
        {
            if (!Directory.Exists(PathMod.APP_PATH + "WIKI/"))
                Directory.CreateDirectory(PathMod.APP_PATH + "WIKI/");

            string endPath = Path.Join(PathMod.APP_PATH, "WIKI/", name + ".txt");
            string endDirectory = Path.GetDirectoryName(endPath);

            if (!Directory.Exists(endDirectory))
                Directory.CreateDirectory(endDirectory);

            if (File.Exists(endPath))
            {
                Console.WriteLine("Path conflict: " + name);
                return false;
            }

            using (var fstream = File.CreateText(endPath))
            {
                fstream.WriteLine(content);

                fstream.Flush();
                fstream.Close();
            }
            return true;
        }

        private static void writeCSVGuide(string name, List<string[]> stats)
        {

            if (!Directory.Exists(PathMod.APP_PATH + "GUIDE/"))
                Directory.CreateDirectory(PathMod.APP_PATH + "GUIDE/");

            using (StreamWriter file = new StreamWriter(PathMod.APP_PATH + "GUIDE/" + name + ".csv"))
            {
                foreach (string[] stat in stats)
                    file.WriteLine(String.Join("\t", stat));
            }

            Console.WriteLine();
        }

        private static void writeHTMLGuide(string name, List<string[]> stats)
        {
            string table =
                "		<table class=\"base-table\">" + 
                "			<thead>" +
                "				<tr>";

            foreach (string title in stats[0])
                table += "					<th scope=\"col\">" + title + "</th>";

            table += "				</tr>" +
                "			</thead>" +
                "			<tbody>";

            for (int ii = 1; ii < stats.Count; ii++)
            {
                table += "				<tr>";
                foreach (string content in stats[ii])
                    table += "					<td>" + content + "</td>";
                table += "				</tr>";
            }

            table += "			</tbody>" +
                "		</table>";

            string html = 
                "<!DOCTYPE html>\n" +
                "<html>\n" +
                "	<head>\n" +
                "		<title>"+name+"</title>\n" +
                "        <meta charset=\"UTF-8\">\n" +
                "        <style type=\"text/css\">\n" +
                "            @import url('https://fonts.googleapis.com/css2?family=Merriweather+Sans:wght@350;700&display=swap');\n" +
                "            @import url('https://fonts.googleapis.com/css2?family=Acme&display=swap');\n" +
                "\n" +
                "            body {\n" +
                "                font-family: \"Merriweather Sans\", \"Tahoma\", sans-serif;\n" +
                "                font-weight: 350;\n" +
                "                margin: 0;\n" +
                "            }\n" +
                "\n" +
                "            header {\n" +
                "                background-color: lightyellow;\n" +
                "                border-bottom: 1px solid #cc9;\n" +
                "                padding: 13px 5px;\n" +
                "                text-align: center;\n" +
                "                color: darkblue;	\n" +
                "            }\n" +
                "            h1 {\n" +
                "                font-size: 32pt;	\n" +
                "                font-weight: bold;	\n" +
                "                font-family: \"Acme\", \"Verdana\", sans-serif;\n" +
                "                margin: 0;\n" +
                "            }\n" +
                "\n" +
                "            nav {\n" +
                "                margin-top: 0.5em;\n" +
                "                font-size: 12pt;\n" +
                "            }\n" +
                "            nav a, nav a:visited {\n" +
                "                margin: 6pt 6pt 0 6pt;\n" +
                "                text-decoration: none;\n" +
                "                border-bottom: none;\n" +
                "                color: darkblue;\n" +
                "                border-bottom-color: lightyellow;\n" +
                "                transition: border-bottom-color 0.2s;\n" +
                "            }\n" +
                "            nav a:hover {\n" +
                "                border-bottom: 1px darkblue solid;\n" +
                "                color: darkblue;\n" +
                "            }\n" +
                "            nav a:active {\n" +
                "                border-bottom: 1px lightskyblue solid;\n" +
                "                color: lightskyblue;\n" +
                "                transition: border-bottom-color 0s;\n" +
                "            }\n" +
                "            nav a.current, nav a.current:hover {\n" +
                "                font-weight: bold;\n" +
                "                border-bottom: none;\n" +
                "            }\n" +
                "\n" +
                "            table.base-table {	\n" +
                "                border: 1px solid #999;	\n" +
                "                font-size: 10pt;	\n" +
                "                width: calc(100% - 16px);\n" +
                "                border-collapse: collapse;\n" +
                "                margin: 8px;\n" +
                "            }\n" +
                "            .base-table td {\n" +
                "                padding: 5px;\n" +
                "                margin: 3px;\n" +
                "                /*text-align: center;*/\n" +
                "            }\n" +
                "            .base-table tr:nth-child(odd)\n" +
                "            {\n" +
                "                background-color: #ddd;\n" +
                "                transition: background-color 0.2s;\n" +
                "            }\n" +
                "            .base-table tr:nth-child(even)\n" +
                "            {\n" +
                "                background-color: #fff;\n" +
                "                transition: background-color 0.2s;\n" +
                "            }\n" +
                "            .base-table tr:hover {\n" +
                "                background-color: #c9d5f0;\n" +
                "                transition: background-color 0.2s;\n" +
                "            }\n" +
                "            .base-table th {\n" +
                "                padding: 5px;\n" +
                "                margin: 3px;\n" +
                "                background-color: #104E8B;\n" +
                "                border-bottom: 1px #013e73 solid;\n" +
                "                color: #FFF;	\n" +
                "                font-weight: bold;	\n" +
                "                text-align: left;\n" +
                "                top: 0;\n" +
                "                position: sticky;\n" +
                "            }\n" +
                "\n" +
                "            footer {	\n" +
                "                background-color:lightgreen;\n" +
                "                border-top: 1px #9c9 solid;	\n" +
                "                color:black;	\n" +
                "                font-style: italic;	\n" +
                "                font-size: 8pt;		\n" +
                "                text-align: right;	\n" +
                "                padding: 8px 5px;\n" +
                "            }	\n" +
                "            </style>\n" +
                "	</head>\n" +
                "	<body>\n" +
                "        <header>\n" +
                "		    <h1>"+name+"</h1>\n" +
                "            <nav>\n" +
                "                <a href=\"Moves.html\" " + currentIfEqual(name, "Moves") + ">Moves</a>\n" +
                "                <a href=\"Items.html\" " + currentIfEqual(name, "Items") + ">Items</a>\n" +
                "                <a href=\"Abilities.html\" " + currentIfEqual(name, "Abilities") + ">Abilities</a>\n" +
                "                <a href=\"Encounters.html\" " + currentIfEqual(name, "Encounters") + ">Encounters</a>\n" +
                "            </nav>\n" +
                "        </header>\n" +
                "		<br>\n" +

                table +

                "		<br>\n" +
                "		<footer>PMDC v"+ Versioning.GetVersion().ToString() + "</footer>\n" +
                "	</body>\n" +
                "</html>\n";
            if (!Directory.Exists(PathMod.APP_PATH + "GUIDE/"))
                Directory.CreateDirectory(PathMod.APP_PATH + "GUIDE/");

            using (StreamWriter file = new StreamWriter(PathMod.APP_PATH + "GUIDE/" + name + ".html"))
                file.Write(html);

            Console.WriteLine();
        }
        
        public static string currentIfEqual(string name, string title)
        {
            return name == title ? "class=\"current\"" : "";
        }

        public static void PrintItemGuide(bool csv)
        {
            List<string[]> stats = new List<string[]>();
            stats.Add(new string[5] { "###", "Name", "Type", "Price", "Description" });
            List<string> itemKeys = DataManager.Instance.DataIndices[DataManager.DataType.Item].GetOrderedKeys(true);
            for (int ii = 0; ii < itemKeys.Count; ii++)
            {
                ProgressBar("Creating item guide...", "Done.", TOTAL_CHUNKS, ii, itemKeys.Count);
                string key = itemKeys[ii];
                ItemData entry = DataManager.Instance.GetItem(key);
                if (entry.Released)
                    stats.Add(new string[5] { key, entry.Name.ToLocal(), entry.UsageType.ToString(), entry.Price.ToString(), entry.Desc.ToLocal() });
            }

            if (csv)
                writeCSVGuide("Items", stats);
            else
                writeHTMLGuide("Items", stats);
        }

        private static bool hasUnown(string input)
        {
            bool hasUnown = false;
            foreach (char c in input)
            {
                if (c > '\uE000')
                {
                    hasUnown = true;
                    break;
                }
            }
            return hasUnown;
        }

        private static string substituteUnown(string input)
        {
            string output = "";
            foreach (char c in input)
            {
                if (c > '\uE000')
                {
                    output += (char)(c - '\uE000');
                }
                else
                    output += c;
            }
            return output;
        }

        public static void PrintItemWiki()
        {
            List<string> itemKeys = DataManager.Instance.DataIndices[DataManager.DataType.Item].GetOrderedKeys(true);
            for (int ii = 0; ii < itemKeys.Count; ii++)
            {
                ProgressBar("Creating item pages...", "Done.", TOTAL_CHUNKS, ii, itemKeys.Count);
                string key = itemKeys[ii];
                ItemData entry = DataManager.Instance.GetItem(key);
                if (entry.Released)
                {
                    string localName = entry.Name.ToLocal();
                    if (hasUnown(localName))
                        localName = substituteUnown(localName);
                    string fileContent = "{{{{{1|ItemData}}}" +
                        "\r\n|item_name=" + localName +
                        "\r\n|sprite=" + entry.Sprite + ".png" +
                        "\r\n|item_id=" + key +
                        "\r\n|is_edible=" + (entry.ItemStates.Contains<EdibleState>() ? "Yes" : "No") +
                        "\r\n|stack_size=" + Math.Max(1, entry.MaxStack) +
                        "\r\n|value=" + entry.Price +
                        "\r\n}}";

                    bool completed = WriteToWiki(localName + "/Data", fileContent);
                    if (!completed)
                        completed = WriteToWiki(localName + " (Item)/Data", fileContent);
                }
            }
        }
        public static void PrintAbilityWiki()
        {
            List<string> abilityKeys = DataManager.Instance.DataIndices[DataManager.DataType.Intrinsic].GetOrderedKeys(true);
            for (int ii = 0; ii < abilityKeys.Count; ii++)
            {
                ProgressBar("Creating ability pages...", "Done.", TOTAL_CHUNKS, ii, abilityKeys.Count);
                string key = abilityKeys[ii];
                IntrinsicData entry = DataManager.Instance.GetIntrinsic(key);
                if (entry.Released)
                {
                    string localName = entry.Name.ToLocal();
                    string fileContent = "{{{{{1|AbilityData}}}" +
                        "\r\n|ability_name=" + localName +
                        "\r\n|ability_id=" + key +
                        "\r\n|description=" + entry.Desc.ToLocal() +
                        "\r\n}}";

                    bool completed = WriteToWiki(localName + "/Data", fileContent);
                    if (!completed)
                        completed = WriteToWiki(localName + " (Ability)/Data", fileContent);
                }
            }
        }

        public static void PrintMonsterWiki()
        {
            Dictionary<string, string> encounterDict = PrintEncounterWiki();

            List<string> itemKeys = DataManager.Instance.DataIndices[DataManager.DataType.Monster].GetOrderedKeys(true);
            for (int ii = 0; ii < itemKeys.Count; ii++)
            {
                string key = itemKeys[ii];
                MonsterData entry = DataManager.Instance.GetMonster(key);
                if (entry.Released && entry.IndexNum > 0)
                {
                    // Get the Pokemon name
                    string localName = entry.Name.ToLocal();
                    int lastValidForm = 0;
                    
                    for (int form = 0; form < entry.Forms.Count; form++)
                    {
                        MonsterFormData formData = (MonsterFormData)entry.Forms[form];
                        // Check if this is a cosmetic form
                        bool formIsCosmetic = false;
                        if (form > 0)
                        {
                            formIsCosmetic = EvaluateCosmeticForm(entry, form, lastValidForm);
                        }
                        // Console.WriteLine(localName + "_" + form + ": " + formIsCosmetic);

                        if (!formIsCosmetic && formData.Released)
                        {
                            // Set the last form used for comparison to cosmetic formes
                            lastValidForm = form;

                            string formName = formData.FormName.DefaultText;
                            string strippedName = formName.Replace(".", "").Replace(":", "").Replace("?", "Question Mark").Replace("!", "Exclamation Mark").Replace("%", " Percent").Replace(" ", "_");


                            // Get type names
                            ElementData element1 = DataManager.Instance.GetElement(formData.Element1);
                            ElementData element2 = DataManager.Instance.GetElement(formData.Element2);

                            // Get ability names
                            IntrinsicData intrinsic1 = DataManager.Instance.GetIntrinsic(formData.Intrinsic1);
                            IntrinsicData intrinsic2 = DataManager.Instance.GetIntrinsic(formData.Intrinsic2);
                            IntrinsicData intrinsic3 = DataManager.Instance.GetIntrinsic(formData.Intrinsic3);

                            // Create main Pokemon data entry
                            string dataFileContent = "{{{{{1|PokemonData}}}";
                            dataFileContent += "\r\n|pokemon_name=" + formName;
                            dataFileContent += "\r\n|pokemon_id=" + key;
                            if (entry.Forms.Count > 1)
                            {
                                dataFileContent += "\r\n|form_id=" + form;
                            }
                            dataFileContent += "\r\n|type1=" + element1.Name.DefaultText;
                            if (element2.Name.DefaultText != "None")
                            {
                                dataFileContent += "\r\n|type2=" + element2.Name.DefaultText;
                            }
                            dataFileContent += "\r\n|ability1=" + intrinsic1.Name.DefaultText;
                            if (intrinsic2.Name.DefaultText != "None")
                            {
                                dataFileContent += "\r\n|ability2=" + intrinsic2.Name.DefaultText;
                            }
                            if (intrinsic3.Name.DefaultText != "None")
                            {
                                dataFileContent += "\r\n|ability3=" + intrinsic3.Name.DefaultText;
                            }
                            dataFileContent += "\r\n|recruit=" + entry.JoinRate;
                            dataFileContent += "\r\n|portrait=Portrait_" + strippedName + ".png";
                            dataFileContent += "\r\n}}";

                            // Write main Pokemon data entry
                            bool completed = WriteToWiki(strippedName + "/Data", dataFileContent);
                            if (!completed) // Check for duplicate form name and append form number as a fallback
                                completed = WriteToWiki(strippedName + "_" + form + "/Data", dataFileContent);


                            // Write learnset data

                            // Level-up learnset
                            string learnsetFileContent = "<h6>By level up</h6>\n{|- class=\"wikitable\"\n{{LearnsetHeader}}\r\n";
                            for (int skill_index = 0; skill_index < formData.LevelSkills.Count; skill_index++)
                            {
                                LevelUpSkill level_up_skill = formData.LevelSkills[skill_index];
                                SkillData current_skill = DataManager.Instance.GetSkill(level_up_skill.Skill);
                                learnsetFileContent += ("|  " + level_up_skill.Level + " {{:" + current_skill.Name.DefaultText + "/Data|LearnsetRow}}\r\n");
                            }
                            // TM learnset
                            learnsetFileContent += "|}\r\n\r\n<h6>By TM</h6>\r\n{|- class=\"wikitable\"\r\n{{LearnsetHeader}}\r\n";
                            for (int skill_index = 0; skill_index < formData.TeachSkills.Count; skill_index++)
                            {
                                LearnableSkill learnable_skill = formData.TeachSkills[skill_index];
                                SkillData current_skill = DataManager.Instance.GetSkill(learnable_skill.Skill);
                                learnsetFileContent += ("| {{:" + current_skill.Name.DefaultText + "/Data|LearnsetRow}}\r\n");
                            }
                            // Tutor learnset
                            learnsetFileContent += "|}\r\n\r\n<h6>By Move Tutor</h6>\r\n{|- class=\"wikitable\"\r\n{{LearnsetHeader}}\r\n";
                            for (int skill_index = 0; skill_index < formData.SecretSkills.Count; skill_index++)
                            {
                                LearnableSkill learnable_skill = formData.SecretSkills[skill_index];
                                SkillData current_skill = DataManager.Instance.GetSkill(learnable_skill.Skill);
                                learnsetFileContent += ("| {{:" + current_skill.Name.DefaultText + "/Data|LearnsetRow}}\r\n");
                            }
                            // Tutor learnset
                            learnsetFileContent += "|}\r\n\r\n<h6>Egg Moves</h6>\r\n{|- class=\"wikitable\"\r\n{{LearnsetHeader}}\r\n";
                            for (int skill_index = 0; skill_index < formData.SharedSkills.Count; skill_index++)
                            {
                                LearnableSkill learnable_skill = formData.SharedSkills[skill_index];
                                SkillData current_skill = DataManager.Instance.GetSkill(learnable_skill.Skill);
                                learnsetFileContent += ("| {{:" + current_skill.Name.DefaultText + "/Data|LearnsetRow}}\r\n");
                            }
                            learnsetFileContent += "|}\r\n\r\n<noinclude>[[Category: Learnsets]]</noinclude>";

                            // Write main Pokemon data entry
                            bool learnset_completed = WriteToWiki(strippedName + "/Learnset", learnsetFileContent);
                            if (!learnset_completed) // Check for duplicate form name and append form number as a fallback
                                learnset_completed = WriteToWiki(strippedName + "_" + form + "/Learnset", learnsetFileContent);


                            // Write stats entry
                            string statsFileContent = "{{StatBars|" +
                                "\r\n|hp=" + formData.BaseHP +
                                "\r\n|atk=" + formData.BaseAtk +
                                "\r\n|def=" + formData.BaseDef +
                                "\r\n|spa=" + formData.BaseMAtk +
                                "\r\n|spd=" + formData.BaseMDef +
                                "\r\n|spe=" + formData.BaseSpeed +
                                "\r\n}}\n<noinclude>[[Category: Pokémon stat pages]]</noinclude>";

                            bool stats_completed = WriteToWiki(strippedName + "/Stats", statsFileContent);
                            if (!stats_completed) // Check for duplicate form name and append form number as a fallback
                                stats_completed = WriteToWiki(strippedName + "_" + form + "/Stats", statsFileContent);

                            // Write locations entry
                            if (encounterDict.ContainsKey(formName))
                            { 
                                string locationFileContent = encounterDict[formName];

                                bool location_completed = WriteToWiki(strippedName + "/Location", locationFileContent);
                                if (!location_completed) // Check for duplicate form name and append form number as a fallback
                                { 
                                    if (encounterDict.ContainsKey(formName + "_" + form))
                                    {
                                        locationFileContent = encounterDict[formName + "_" + form];
                                        location_completed = WriteToWiki(strippedName + "_" + form + "/Location", locationFileContent);
                                    }
                                    else
                                    {
                                        locationFileContent = "N/A";
                                        location_completed = WriteToWiki(strippedName + "_" + form + "/Location", locationFileContent);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<string, string> PrintEncounterWiki()
        {
            Dictionary<string, string> encounterDict = new Dictionary<string, string>();

            List<string> monsterKeys = DataManager.Instance.DataIndices[DataManager.DataType.Monster].GetOrderedKeys(true);
            ProgressBar("Creating encounters guide...", "Done.", TOTAL_CHUNKS, 0, monsterKeys.Count);

            Dictionary<MonsterID, HashSet<(string tag, ZoneLoc encounter)>> foundSpecies = DevHelper.GetAllAppearingMonsters(true);

            foreach (StartChar startchar in DataManager.Instance.Start.Chars)
                DevHelper.AddWithEvos(foundSpecies, new MonsterID(startchar.ID.Species, startchar.ID.Form, "", Gender.Unknown), "STARTER", ZoneLoc.Invalid);

            for (int ii = 0; ii < monsterKeys.Count; ii++)
            {
                ProgressBar("Creating encounters guide...", "Done.", TOTAL_CHUNKS, ii, monsterKeys.Count);
                string key = monsterKeys[ii];
                MonsterEntrySummary summary = (MonsterEntrySummary)DataManager.Instance.DataIndices[DataManager.DataType.Monster].Get(key);
                MonsterData data = DataManager.Instance.GetMonster(key);
                int formIndexNumber = 0;
                for (int jj = 0; jj < summary.Forms.Count; jj++)
                {
                    MonsterFormData formData = (MonsterFormData)data.Forms[jj];
                    if (formData.Temporary)
                        continue;

                    string encounterStr = "UNKNOWN";
                    if (summary.Released && formData.Released)
                    {
                        MonsterID monId = new MonsterID(key, jj, "", Gender.Unknown);
                        if (foundSpecies.ContainsKey(monId))
                        {
                            bool evolve = false;
                            bool starter = false;

                            Dictionary<string, (Dictionary<string, HashSet<int>> specialDict, Dictionary<string, Dictionary<int, HashSet<int>>> floorDict)> foundDict = new Dictionary<string, (Dictionary<string, HashSet<int>> specialDict, Dictionary<string, Dictionary<int, HashSet<int>>> floorDict)>();

                            foreach ((string tag, ZoneLoc encounter) in foundSpecies[monId])
                            {
                                if (!foundDict.ContainsKey(tag))
                                    foundDict[tag] = (new Dictionary<string, HashSet<int>>(), new Dictionary<string, Dictionary<int, HashSet<int>>>());
                                Dictionary<string, HashSet<int>> specialDict = foundDict[tag].specialDict;
                                Dictionary<string, Dictionary<int, HashSet<int>>> floorDict = foundDict[tag].floorDict;

                                if (tag == "STARTER")
                                    starter = true;
                                else if (tag == "EVOLVE")
                                    evolve = true;
                                else if (encounter.StructID.ID == -1)
                                {
                                    if (!specialDict.ContainsKey(encounter.ID))
                                        specialDict[encounter.ID] = new HashSet<int>();
                                    specialDict[encounter.ID].Add(encounter.StructID.Segment);
                                }
                                else
                                {
                                    if (!floorDict.ContainsKey(encounter.ID))
                                        floorDict[encounter.ID] = new Dictionary<int, HashSet<int>>();
                                    if (!floorDict[encounter.ID].ContainsKey(encounter.StructID.Segment))
                                        floorDict[encounter.ID][encounter.StructID.Segment] = new HashSet<int>();
                                    floorDict[encounter.ID][encounter.StructID.Segment].Add(encounter.StructID.ID);
                                }
                            }

                            List<string> encounterMsg = new List<string>();

                            foreach (string tag in foundDict.Keys)
                            {
                                Dictionary<string, HashSet<int>> specialDict = foundDict[tag].specialDict;
                                Dictionary<string, Dictionary<int, HashSet<int>>> floorDict = foundDict[tag].floorDict;

                                foreach (string zz in DataManager.Instance.DataIndices[DataManager.DataType.Zone].GetOrderedKeys(true))
                                {
                                    ZoneData mainZone = DataManager.Instance.GetZone(zz);
                                    for (int yy = 0; yy < mainZone.Segments.Count; yy++)
                                    {
                                        if (specialDict.ContainsKey(zz) && specialDict[zz].Contains(yy))
                                        {
                                            string locString = String.Format("{0} {1}S", mainZone.Name.ToLocal(), yy + 1);
                                            string formattedZoneName = mainZone.Name.ToLocal();
                                            foreach (var step in mainZone.Segments[yy].ZoneSteps)
                                            {
                                                var startStep = step as FloorNameIDZoneStep;
                                                if (startStep != null)
                                                {
                                                    locString = LocalText.FormatLocalText(startStep.Name, "?").ToLocal().Replace('\n', ' ');
                                                    break;
                                                }
                                            }
                                            locString = locString.Replace(mainZone.Name.ToLocal(), formattedZoneName);
                                            if (tag != "")
                                                locString = String.Format("[{0}] [[{1}", tag, locString);
                                            else
                                                locString = "[[" + locString;
                                            int place = locString.LastIndexOf(" ");
                                            locString = locString.Remove(place, 1).Insert(place, "]] ");
                                            encounterMsg.Add(locString);
                                        }

                                        if (floorDict.ContainsKey(zz) && floorDict[zz].ContainsKey(yy))
                                        {
                                            List<string> ranges = combineFloorRanges(floorDict[zz][yy]);
                                            string rangeString = String.Join(",", ranges.ToArray());
                                            string formattedZoneName = mainZone.Name.ToLocal();
                                            string locString = String.Format("{0} {1}S {2}F", formattedZoneName, yy + 1, rangeString);
                                            foreach (var step in mainZone.Segments[yy].ZoneSteps)
                                            {
                                                var startStep = step as FloorNameIDZoneStep;
                                                if (startStep != null)
                                                {
                                                    locString = LocalText.FormatLocalText(startStep.Name, rangeString).ToLocal().Replace('\n', ' ');
                                                    break;
                                                }
                                            }
                                            locString = locString.Replace(mainZone.Name.ToLocal(), formattedZoneName);
                                            if (tag != "")
                                                locString = String.Format("[{0}] [[{1}", tag, locString);
                                            else
                                                locString = "[[" + locString;

                                            int place = locString.LastIndexOf(" ");
                                            locString = locString.Remove(place, 1).Insert(place, "]] ");

                                            encounterMsg.Add(locString);
                                        }
                                    }
                                }
                            }

                            if (evolve && encounterMsg.Count == 0)
                                encounterMsg.Add("Evolve");
                            else if (starter && encounterMsg.Count == 0)
                                encounterMsg.Add("Starter");

                            if (encounterMsg.Count > 0)
                                encounterStr = String.Join("\n", encounterMsg.ToArray());
                        }
                    }
                    else
                    {
                        encounterStr = "NO DATA";
                    }
                    string monsterName = formData.FormName.ToLocal();
                    if (encounterDict.ContainsKey(monsterName))
                    {
                        formIndexNumber = formIndexNumber + 1;
                        monsterName = monsterName + "_" + formIndexNumber.ToString();
                    }
                    else
                    {
                        formIndexNumber = 0;
                    }
                    //Console.WriteLine(monsterName + "\n " + encounterStr + "\n\n");
                    encounterDict.Add(monsterName, encounterStr);
                }
            }

            return encounterDict;
        }

        public static List<MonsterFormData> EvaluateMonsterEvolution(MonsterData startingMonster, int baseForm, List<PromoteBranch> evolutionBranches)
        {
            List<MonsterFormData> validEvolutionForms = new List<MonsterFormData>();

            for (int evolutionBranchIndex = 0; evolutionBranchIndex < evolutionBranches.Count; evolutionBranchIndex++)
            {
                PromoteBranch evolutionBranch = evolutionBranches[evolutionBranchIndex];
                MonsterData evolvedMonster = DataManager.Instance.GetMonster(evolutionBranch.Result);
                int lastValidForm = 0;

                for (int formIndex = 0; formIndex < evolvedMonster.Forms.Count; formIndex++)
                {
                    BaseMonsterForm evolvedForm = evolvedMonster.Forms[formIndex];

                    if (evolvedForm.PromoteForm == baseForm)
                    {
                        // Check if this form is purely cosmetic
                        bool formIsCosmetic = false;
                        if (formIndex > 0)
                        {
                            formIsCosmetic = EvaluateCosmeticForm(evolvedMonster, formIndex, lastValidForm);
                        }
                        if (!formIsCosmetic)
                        {
                            validEvolutionForms.Add((MonsterFormData)evolvedForm);
                            lastValidForm = formIndex;
                        }

                        // Check this monster's evolved forms
                        List<PromoteBranch> secondEvolutionBranches = evolvedMonster.Promotions;

                        for (int secondEvolutionBranchIndex = 0; secondEvolutionBranchIndex < secondEvolutionBranches.Count; secondEvolutionBranchIndex++)
                        {
                            PromoteBranch secondEvolutionBranch = secondEvolutionBranches[secondEvolutionBranchIndex];
                            MonsterData secondEvolvedMonster = DataManager.Instance.GetMonster(secondEvolutionBranch.Result);
                            int lastValidSecondForm = 0;

                            for (int secondFormIndex = 0; secondFormIndex < secondEvolvedMonster.Forms.Count; secondFormIndex++)
                            {
                                BaseMonsterForm secondEvolvedForm = secondEvolvedMonster.Forms[secondFormIndex];
                                if (secondEvolvedForm.PromoteForm == baseForm)
                                {
                                    // Check if this form is purely cosmetic
                                    bool secondFormIsCosmetic = false;
                                    if (secondFormIndex > 0)
                                    {
                                        secondFormIsCosmetic = EvaluateCosmeticForm(secondEvolvedMonster, secondFormIndex, lastValidSecondForm);
                                    }
                                    if (!secondFormIsCosmetic)
                                    {
                                        validEvolutionForms.Add((MonsterFormData)secondEvolvedForm);
                                        lastValidSecondForm = secondFormIndex;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return validEvolutionForms;
        }

        public static bool EvaluateCosmeticForm(MonsterData evaluatedMonster, int evaluatedFormIndex, int baseFormIndex = 0)
        {
            // Check if this form is purely cosmetic
            bool formIsCosmetic = false;
            MonsterFormData baseEvolvedData = (MonsterFormData)evaluatedMonster.Forms[baseFormIndex];
            MonsterFormData evolvedData = (MonsterFormData)evaluatedMonster.Forms[evaluatedFormIndex];

            // Compare types to see if they're identical
            bool identicalElements = false;
            if ((baseEvolvedData.Element1 == evolvedData.Element1) && (baseEvolvedData.Element2 == evolvedData.Element2))
            {
                identicalElements = true;
            }

            // Compare abilities to see if they're identical
            bool identicalAbilities = false;
            if ((baseEvolvedData.Intrinsic1 == evolvedData.Intrinsic1) && (baseEvolvedData.Intrinsic2 == evolvedData.Intrinsic2) && (baseEvolvedData.Intrinsic3 == evolvedData.Intrinsic3))
            {
                identicalAbilities = true;
            }

            // Compare stats to see if they're identical
            bool identicalStats = false;

            if ((baseEvolvedData.BaseAtk == evolvedData.BaseAtk) && (baseEvolvedData.BaseDef == evolvedData.BaseDef) && (baseEvolvedData.BaseMAtk == evolvedData.BaseMAtk) && (baseEvolvedData.BaseMDef == evolvedData.BaseMDef) && (baseEvolvedData.BaseSpeed == evolvedData.BaseSpeed))
            {
                identicalStats = true;
            }

            if (identicalElements && identicalAbilities && identicalStats)
            {
                formIsCosmetic = true;
            }

            return formIsCosmetic;
        }

        public static void PrintMonsterFamilyWiki()
        {
            // Get a list of first-form Pokemon to serve as the evolution tree's roots
            List<string> itemKeys = DataManager.Instance.DataIndices[DataManager.DataType.Monster].GetOrderedKeys(true);
            List<MonsterData> firstFormMonsters = new List<MonsterData>();
            for (int ii = 0; ii < itemKeys.Count; ii++)
            {
                string key = itemKeys[ii];
                MonsterData entry = DataManager.Instance.GetMonster(key);
                if (entry.Released)
                {
                    if (entry.PromoteFrom == "")
                    {
                        if (entry.IndexNum > 0)
                        {
                            firstFormMonsters.Add(entry);
                        }
                    }
                }
            }

            Console.WriteLine("Begin printing Pokemon families");
            // For each Pokemon, create lists containing each form's evolution tree
            for (int ii = 0; ii < firstFormMonsters.Count; ii++)
            {
                ProgressBar("Creating monster family pages...", "Done.", TOTAL_CHUNKS, ii, firstFormMonsters.Count);
                List<List<MonsterFormData>> monsterFamilyData = new List<List<MonsterFormData>>();

                // Get the base form
                MonsterData startingMonster = firstFormMonsters[ii];

                bool singleStageFamily = true;
                int lastValidForm = 0;

                for (int form = 0; form < startingMonster.Forms.Count; form++)
                {
                    bool formIsCosmetic = false;
                    if (form > 0)
                    {
                        formIsCosmetic = EvaluateCosmeticForm(startingMonster, form, lastValidForm);
                    }
                    if (!formIsCosmetic)
                    {
                        // Set this form as the one to compare stats to
                        lastValidForm = form;

                        List<MonsterFormData> currentEvolutionBranch = new List<MonsterFormData>();
                        currentEvolutionBranch.Add((MonsterFormData)startingMonster.Forms[form]);

                        // Get list of valid evolutions
                        List<MonsterFormData> validEvolutions = EvaluateMonsterEvolution(startingMonster, form, startingMonster.Promotions);
                        for (int validEvolutionIndex = 0; validEvolutionIndex < validEvolutions.Count; validEvolutionIndex++)
                        {
                            singleStageFamily = false;
                            MonsterFormData currentEvolution = validEvolutions[validEvolutionIndex];
                            if (currentEvolution.Released)
                            {
                                if (currentEvolutionBranch.IndexOf(currentEvolution) == -1)
                                {
                                    currentEvolutionBranch.Add(currentEvolution);
                                }
                            }
                        }

                        // Add this branch of the family tree to the family list
                        monsterFamilyData.Add(currentEvolutionBranch);
                    }
                }

                // Keep track of names that have already been used in the data structure
                List<String> namesAlreadyUsed = new List<string>();
                List<String> redirectNames = new List<string>();
                int currentFormNumber = 0;

                // Print the Pokemon family page
                string fileContent = "__NOTOC__";

                for (int evolutionBranchIndex = 0; evolutionBranchIndex < monsterFamilyData.Count; evolutionBranchIndex++)
                {
                    // Create tabs for each evolution branch
                    fileContent += "\r\n\r\n<tabs>";

                    // For each Pokemon in the branch, add a tab for it
                    for(int familyMemberIndex = 0; familyMemberIndex < monsterFamilyData[evolutionBranchIndex].Count; familyMemberIndex++)
                    {
                        MonsterFormData currentMonsterForm = monsterFamilyData[evolutionBranchIndex][familyMemberIndex];

                        string formName = currentMonsterForm.FormName.DefaultText;
                        string strippedName = formName.Replace(".", "").Replace(":", "").Replace("?", "Question Mark").Replace("!", "Exclamation Mark").Replace("%", " Percent").Replace(" ", "_");

                        if (namesAlreadyUsed.Contains(strippedName))
                        {
                            currentFormNumber += 1;
                            strippedName = strippedName + "_" + currentFormNumber.ToString();
                        }
                        else
                        {
                            currentFormNumber = 0;
                        }

                        fileContent += ("\r\n<tab name=\"" + formName + "\">{{:" + strippedName + "/Data|PokemonInfobox}}</tab>");

                        namesAlreadyUsed.Add(strippedName);
                        redirectNames.Add(formName);
                    }

                    // End the tab
                    fileContent += "\r\n</tabs>";
                }
                /*
                foreach (string nameUsed in namesAlreadyUsed)
                {
                    Console.WriteLine(nameUsed);
                }
                */
                fileContent += "\r\n";

                // Write to file
                string firstFormStrippedName = startingMonster.Name.DefaultText;
                firstFormStrippedName = firstFormStrippedName.Replace(".", "").Replace(":", "").Replace("?", "Question Mark").Replace("!", "Exclamation Mark").Replace("%", " Percent").Replace(" ", "_");
                if (!singleStageFamily)
                {
                    firstFormStrippedName += "_family";
                }

                bool completed = WriteToWiki(firstFormStrippedName, fileContent);
                if (!completed) // Check for duplicate form name and append form number as a fallback
                    completed = WriteToWiki(firstFormStrippedName + " (Pokemon)", fileContent);

                // Create redirect pages
                if (!singleStageFamily)
                {
                    for (int redirectNameIndex = 0; redirectNameIndex < namesAlreadyUsed.Count; redirectNameIndex++)
                    {
                        if (redirectNameIndex == 0)
                        {
                            WriteToWiki(namesAlreadyUsed[redirectNameIndex], "#REDIRECT [[" + firstFormStrippedName.Replace("_", " ") + "]]");
                        }
                        else
                        {
                            WriteToWiki(namesAlreadyUsed[redirectNameIndex], "#REDIRECT [[" + firstFormStrippedName.Replace("_", " ") + "#" + redirectNames[redirectNameIndex] + "]]");
                        }
                    }
                }
            }
        }

        public static void PrintMoveGuide(bool csv)
        {
            List<string[]> stats = new List<string[]>();
            stats.Add(new string[9] { "###", "Name", "Type", "Category", "Power", "Accuracy", "PP", "Range", "Description" });
            List<string> moves = DataManager.Instance.DataIndices[DataManager.DataType.Skill].GetOrderedKeys(true);
            for (int ii = 0; ii < moves.Count; ii++)
            {
                ProgressBar("Creating moves guide...", "Done.", TOTAL_CHUNKS, ii, moves.Count);
                string key = moves[ii];
                SkillData entry = DataManager.Instance.GetSkill(key);
                if (entry.Released)
                {
                    ElementData elementEntry = DataManager.Instance.GetElement(entry.Data.Element);
                    BasePowerState powerState = entry.Data.SkillStates.GetWithDefault<BasePowerState>();
                    stats.Add(new string[9] { key, entry.Name.ToLocal(),
                        elementEntry.Name.ToLocal(),
                        entry.Data.Category.ToLocal(),
                        powerState != null ? powerState.Power.ToString() : "---",
                        entry.Data.HitRate.ToString(),
                        entry.BaseCharges.ToString(),
                        entry.HitboxAction.GetDescription(),
                        entry.Desc.ToLocal()});
                }
                else
                    stats.Add(new string[9] { key, entry.Name.ToLocal(), "???", "None", "---", "---", "N/A", "No One", "NO DATA" });
                //effect chance
                //additional flags
            }
            if (csv)
                writeCSVGuide("Moves", stats);
            else
                writeHTMLGuide("Moves", stats);
        }

        public static void PrintMoveWiki()
        {
            List<string> itemKeys = DataManager.Instance.DataIndices[DataManager.DataType.Skill].GetOrderedKeys(true);
            for (int ii = 0; ii < itemKeys.Count; ii++)
            {
                ProgressBar("Creating skill pages...", "Done.", TOTAL_CHUNKS, ii, itemKeys.Count);
                string key = itemKeys[ii];
                SkillData entry = DataManager.Instance.GetSkill(key);
                if (entry.Released)
                {
                    string localName = entry.Name.ToLocal();
                    string localDesc = entry.Desc.ToLocal();
                    ElementData elementEntry = DataManager.Instance.GetElement(entry.Data.Element);
                    BasePowerState powerState = entry.Data.SkillStates.GetWithDefault<BasePowerState>();
                    string target_string = entry.HitboxAction.GetTargetsString(false);
                    string target_string_plural = entry.HitboxAction.GetTargetsString(true);
                    string true_target_string = "";
                    string range_string = entry.HitboxAction.GetDescription();
                    if (range_string.StartsWith(target_string_plural + " in "))
                        true_target_string = target_string_plural;
                    else if (range_string.StartsWith(target_string + " in "))
                        true_target_string = target_string;
                    if (true_target_string != "")
                        range_string = range_string.Substring(true_target_string.Length + 4, range_string.Length - true_target_string.Length - 4);
                    string power_string = (powerState != null ? powerState.Power.ToString() : "--");
                    if (entry.Strikes > 1)
                        power_string += "x" + entry.Strikes;
                    string hit_string = (entry.Data.HitRate > 0 ? entry.Data.HitRate.ToString() : "--");
                    List<string> removals = new List<string>();
                    foreach (BattleEvent battleEvent in entry.Data.OnHitTiles.EnumerateInOrder())
                    {
                        if (battleEvent is RemoveItemEvent)
                            removals.Add("Destroys Items");
                        else if (battleEvent is RemoveTrapEvent)
                            removals.Add("Destroys Traps");
                        else if (battleEvent is RemoveTerrainStateEvent)
                        {
                            RemoveTerrainStateEvent removeTerrain = (RemoveTerrainStateEvent)battleEvent;
                            foreach (FlagType state in removeTerrain.States)
                            {
                                if (state.FullType == typeof(WallTerrainState))
                                    removals.Add("Breaks Walls");
                                else if (state.FullType == typeof(WaterTerrainState))
                                    removals.Add("Removes Water");
                                else if (state.FullType == typeof(LavaTerrainState))
                                    removals.Add("Removes Lava");
                                else if (state.FullType == typeof(AbyssTerrainState))
                                    removals.Add("Removes Pits");
                                else if (state.FullType == typeof(FoliageTerrainState))
                                    removals.Add("Removes Grass");
                            }
                        }
                        else if (battleEvent is ShatterTerrainEvent)
                        {
                            ShatterTerrainEvent removeTerrain = (ShatterTerrainEvent)battleEvent;
                            foreach (string state in removeTerrain.TileTypes)
                            {
                                if (state == "wall")
                                    removals.Add("Breaks Walls + Adjacents");
                            }
                        }
                    }

                    string terrain_string = "None";
                    if (removals.Count > 0)
                    {
                        terrain_string = String.Join("\n", removals);
                    }

                    string fileContent = "{{{{{1|MoveData}}}" +
                        "\r\n|move_name=" + localName +
                        "\r\n|move_id=" + key +
                        "\r\n|type=" + elementEntry.Name.ToLocal() +
                        "\r\n|category=" + entry.Data.Category.ToLocal() +
                        "\r\n|power=" + power_string +
                        "\r\n|accuracy=" + hit_string +
                        "\r\n|pp=" + entry.BaseCharges +
                        "\r\n|range=" + range_string +
                        "\r\n|target=" + true_target_string +
                        "\r\n|terrain_effects=" + terrain_string +
                        "\r\n|description=" + "[TMP] " + localDesc +
                        "\r\n}}";

                    bool completed = WriteToWiki(localName + "/Data", fileContent);
                    if (!completed)
                        completed = WriteToWiki(localName + " (Move)/Data", fileContent);
                }
            }
        }

        public static void PrintAbilityGuide(bool csv)
        {
            List<string[]> stats = new List<string[]>();
            stats.Add(new string[3] { "###", "Name", "Description" });

            List<string> abilities =
                DataManager.Instance.DataIndices[DataManager.DataType.Intrinsic].GetOrderedKeys(true);
            
            for (int ii = 0; ii < abilities.Count; ii++)
            {
                ProgressBar("Creating abilities guide...", "Done.", TOTAL_CHUNKS, ii, abilities.Count);
                string key = abilities[ii];
                IntrinsicData entry = DataManager.Instance.GetIntrinsic(key);
                if (entry.Released)
                    stats.Add(new string[3] { key, entry.Name.ToLocal(), entry.Desc.ToLocal() });
                else
                    stats.Add(new string[3] { key, entry.Name.ToLocal(), "NO DATA" });
            }

            if (csv)
                writeCSVGuide("Abilities", stats);
            else
                writeHTMLGuide("Abilities", stats);
        }

        private static List<string> combineFloorRanges(HashSet<int> floors)
        {
            List<string> rangeStrings = new List<string>();

            List<int> sortedFloors = new List<int>();
            sortedFloors.AddRange(floors);
            sortedFloors.Sort();

            int curStart = sortedFloors[0];
            int curEnd = sortedFloors[0];
            for (int ii = 1; ii < sortedFloors.Count; ii++)
            {
                int floor = sortedFloors[ii];
                if (floor > curEnd + 1)
                {
                    if (curStart == curEnd)
                        rangeStrings.Add((curStart + 1).ToString());
                    else
                        rangeStrings.Add((curStart + 1).ToString()+"-"+(curEnd + 1).ToString());

                    curStart = floor;
                    curEnd = floor;
                }
                else
                    curEnd = floor;
            }

            if (curStart == curEnd)
                rangeStrings.Add((curStart + 1).ToString());
            else
                rangeStrings.Add((curStart + 1).ToString() + "-" + (curEnd + 1).ToString());
            return rangeStrings;
        }

        public static void PrintEncounterGuide(bool csv)
        {
            List<string> monsterKeys = DataManager.Instance.DataIndices[DataManager.DataType.Monster].GetOrderedKeys(true);
            ProgressBar("Creating encounters guide...", "Done.", TOTAL_CHUNKS, 0, monsterKeys.Count);
            
            Dictionary<MonsterID, HashSet<(string tag, ZoneLoc encounter)>> foundSpecies = DevHelper.GetAllAppearingMonsters(true);

            foreach (StartChar startchar in DataManager.Instance.Start.Chars)
                DevHelper.AddWithEvos(foundSpecies, new MonsterID(startchar.ID.Species, startchar.ID.Form, "", Gender.Unknown), "STARTER", ZoneLoc.Invalid);

            List<string[]> stats = new List<string[]>();
            stats.Add(new string[4] { "###", "Name", "Join %", "Found In" });

            for (int ii = 0; ii < monsterKeys.Count; ii++ )
            {
                ProgressBar("Creating encounters guide...", "Done.", TOTAL_CHUNKS, ii, monsterKeys.Count);
                string key = monsterKeys[ii];
                MonsterEntrySummary summary = (MonsterEntrySummary)DataManager.Instance.DataIndices[DataManager.DataType.Monster].Get(key);
                MonsterData data = DataManager.Instance.GetMonster(key);
                for (int jj = 0; jj < summary.Forms.Count; jj++)
                {
                    MonsterFormData formData = (MonsterFormData)data.Forms[jj];
                    if (formData.Temporary)
                        continue;

                    if (summary.Released && formData.Released)
                    {
                        string encounterStr = "UNKNOWN";
                        MonsterID monId = new MonsterID(key, jj, "", Gender.Unknown);
                        if (foundSpecies.ContainsKey(monId))
                        {
                            bool evolve = false;
                            bool starter = false;

                            Dictionary<string, (Dictionary<string, HashSet<int>> specialDict, Dictionary<string, Dictionary<int, HashSet<int>>> floorDict)> foundDict = new Dictionary<string, (Dictionary<string, HashSet<int>> specialDict, Dictionary<string, Dictionary<int, HashSet<int>>> floorDict)>();

                            foreach ((string tag, ZoneLoc encounter) in foundSpecies[monId])
                            {
                                if (!foundDict.ContainsKey(tag))
                                    foundDict[tag] = (new Dictionary<string, HashSet<int>>(), new Dictionary<string, Dictionary<int, HashSet<int>>>());
                                Dictionary<string, HashSet<int>> specialDict = foundDict[tag].specialDict;
                                Dictionary<string, Dictionary<int, HashSet<int>>> floorDict = foundDict[tag].floorDict;

                                if (tag == "STARTER")
                                    starter = true;
                                else if (tag == "EVOLVE")
                                    evolve = true;
                                else if (encounter.StructID.ID == -1)
                                {
                                    if (!specialDict.ContainsKey(encounter.ID))
                                        specialDict[encounter.ID] = new HashSet<int>();
                                    specialDict[encounter.ID].Add(encounter.StructID.Segment);
                                }
                                else
                                {
                                    if (!floorDict.ContainsKey(encounter.ID))
                                        floorDict[encounter.ID] = new Dictionary<int, HashSet<int>>();
                                    if (!floorDict[encounter.ID].ContainsKey(encounter.StructID.Segment))
                                        floorDict[encounter.ID][encounter.StructID.Segment] = new HashSet<int>();
                                    floorDict[encounter.ID][encounter.StructID.Segment].Add(encounter.StructID.ID);
                                }
                            }

                            List<string> encounterMsg = new List<string>();

                            foreach (string tag in foundDict.Keys)
                            {
                                Dictionary<string, HashSet<int>> specialDict = foundDict[tag].specialDict;
                                Dictionary<string, Dictionary<int, HashSet<int>>> floorDict = foundDict[tag].floorDict;

                                foreach (string zz in DataManager.Instance.DataIndices[DataManager.DataType.Zone].GetOrderedKeys(true))
                                {
                                    ZoneData mainZone = DataManager.Instance.GetZone(zz);
                                    for (int yy = 0; yy < mainZone.Segments.Count; yy++)
                                    {
                                        if (specialDict.ContainsKey(zz) && specialDict[zz].Contains(yy))
                                        {
                                            string locString = String.Format("{0} {1}S", mainZone.Name.ToLocal(), yy + 1);
                                            foreach (var step in mainZone.Segments[yy].ZoneSteps)
                                            {
                                                var startStep = step as FloorNameIDZoneStep;
                                                if (startStep != null)
                                                {
                                                    locString = LocalText.FormatLocalText(startStep.Name, "?").ToLocal().Replace('\n', ' ');
                                                    break;
                                                }
                                            }
                                            if (tag != "")
                                                locString = String.Format("[{0}] {1}", tag, locString);
                                            encounterMsg.Add(locString);
                                        }

                                        if (floorDict.ContainsKey(zz) && floorDict[zz].ContainsKey(yy))
                                        {
                                            List<string> ranges = combineFloorRanges(floorDict[zz][yy]);
                                            string rangeString = String.Join(",", ranges.ToArray());
                                            string locString = String.Format("{0} {1}S {2}F", mainZone.Name.ToLocal(), yy + 1, rangeString);
                                            foreach (var step in mainZone.Segments[yy].ZoneSteps)
                                            {
                                                var startStep = step as FloorNameIDZoneStep;
                                                if (startStep != null)
                                                {
                                                    locString = LocalText.FormatLocalText(startStep.Name, rangeString).ToLocal().Replace('\n', ' ');
                                                    break;
                                                }
                                            }
                                            if (tag != "")
                                                locString = String.Format("[{0}] {1}", tag, locString);
                                            encounterMsg.Add(locString);
                                        }
                                    }
                                }
                            }

                            if (evolve && encounterMsg.Count == 0)
                                encounterMsg.Add("Evolve");
                            else if (starter && encounterMsg.Count == 0)
                                encounterMsg.Add("Starter");

                            if (encounterMsg.Count > 0)
                                encounterStr = String.Join(", ", encounterMsg.ToArray());
                        }
                        stats.Add(new string[4] { summary.SortOrder.ToString("D3"), formData.FormName.ToLocal(), data.JoinRate.ToString() + "%", encounterStr });
                    }
                    else
                        stats.Add(new string[4] { summary.SortOrder.ToString("D3"), formData.FormName.ToLocal(), "--%", "NO DATA" });
                }
            }
            if (csv)
                writeCSVGuide("Encounters", stats);
            else
                writeHTMLGuide("Encounters", stats);
        }

        public static void PrintDungeonEncounterWiki()
        {
            List<string> dungeonList = new List<string>();
            dungeonList = DataManager.Instance.DataIndices[DataManager.DataType.Zone].GetOrderedKeys(true);

            for (int dungeonIndex = 0; dungeonIndex < dungeonList.Count; dungeonIndex++)
            {
                // Create list of dungeon spawns
                List<string[]> dungeonSpawnList = new List<string[]>();
                int conflictSegment = 0;

                // Get the dungeon's main zone
                ZoneData mainZone = DataManager.Instance.GetZone(dungeonList[dungeonIndex]);
                string mainZoneName = mainZone.Name.DefaultText;
                //Console.WriteLine(mainZone.Name.ToLocal());

                List<ZoneSegmentBase> segmentList = mainZone.Segments;
                for (int zoneIndex = 0; zoneIndex < segmentList.Count; zoneIndex++)
                {
                    // Create lists of Pokemon spawns
                    List<DungeonSpawnData> segmentSpawnList = new List<DungeonSpawnData>();
                    List<DungeonSpawnData> specialSpawnList = new List<DungeonSpawnData>();
                    List<DungeonSpawnData> vaultSpawnList = new List<DungeonSpawnData>();
                    List<StaticSpawnData> staticSpawnList = new List<StaticSpawnData>();

                    // Get the current dungeon segment
                    ZoneSegmentBase currentSegment = segmentList[zoneIndex];

                    // Variable to check basement floors
                    bool isBasementFloor = false;

                    // Look through current segment's global steps to find Pokemon spawning step
                    List<ZoneStep> zoneStepList = currentSegment.ZoneSteps;
                    string zoneName = "";
                    string trimmedZoneName = "";
                    foreach (ZoneStep step in zoneStepList)
                    {
                        Type zoneStepType = step.GetType();
                        if (zoneStepType == typeof(FloorNameDropZoneStep))
                        {
                            FloorNameDropZoneStep castZoneStep = (FloorNameDropZoneStep)step;
                            zoneName = castZoneStep.Name.ToLocal();
                            trimmedZoneName = zoneName.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("B{0}F", "").Replace("{0}F", "").TrimEnd().Replace(" ", "_");
                            //Console.WriteLine(trimmedZoneName);

                            if (zoneName.Contains("B{0}"))
                            {
                                isBasementFloor = true;
                            }
                        }
                        if (zoneStepType == typeof(TeamSpawnZoneStep))
                        {
                            TeamSpawnZoneStep castZoneStep = (TeamSpawnZoneStep)step;

                            // Get list of regular dungeon enemies
                            SpawnRangeList<TeamMemberSpawn> spawnList = castZoneStep.Spawns;
                            // Make a data entry for each enemy
                            for(int spawnIndex = 0; spawnIndex < spawnList.Count; spawnIndex++)
                            {
                                // Get current enemy data
                                TeamMemberSpawn currentSpawn = spawnList.GetSpawn(spawnIndex);
                                MobSpawn currentMob = currentSpawn.Spawn;

                                IntRange floorRange = spawnList.GetSpawnRange(spawnIndex);
                                DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, currentSpawn, floorRange.Min, floorRange.Max, null, isBasementFloor);
                                segmentSpawnList.Add(encounterData);
                            }
                        }
                        if (zoneStepType == typeof(SpreadStepRangeZoneStep))
                        {
                            SpreadStepRangeZoneStep castZoneStep = (SpreadStepRangeZoneStep)step;
                            SpawnRangeList<IGenStep> spreadSteps = castZoneStep.Spawns;
                            for (int stepIndex = 0; stepIndex < spreadSteps.Count; stepIndex++)
                            {
                                //Console.WriteLine(spreadSteps.GetSpawn(stepIndex).GetType().GetFormattedTypeName());
                                // Check for placing random mobs
                                if (spreadSteps.GetSpawn(stepIndex).GetType() == typeof(PlaceRandomMobsStep<ListMapGenContext>))
                                {
                                    PlaceRandomMobsStep<ListMapGenContext> mobSpawnStep = (PlaceRandomMobsStep<ListMapGenContext>)spreadSteps.GetSpawn(stepIndex);
                                    if (mobSpawnStep.Spawn.GetType() == typeof(LoopedTeamSpawner<ListMapGenContext>))
                                    { 
                                        LoopedTeamSpawner<ListMapGenContext> teamSpawner = (LoopedTeamSpawner<ListMapGenContext>)mobSpawnStep.Spawn;
                                        SpecificTeamSpawner specificSpawner = (SpecificTeamSpawner)teamSpawner.Picker;
                                        List<MobSpawn> specificSpawns = specificSpawner.Spawns;

                                        for(int specificSpawnIndex = 0; specificSpawnIndex < specificSpawns.Count; specificSpawnIndex++)
                                        {
                                            MobSpawn currentMob = specificSpawns[specificSpawnIndex];
                                            DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, null, castZoneStep.SpreadPlan.FloorRange.Min, castZoneStep.SpreadPlan.FloorRange.Max + 1, null, isBasementFloor);
                                            specialSpawnList.Add(encounterData);
                                        }
                                    }
                                }
                            }
                        }
                        if (zoneStepType == typeof(SpreadVaultZoneStep))
                        {
                            SpreadVaultZoneStep castZoneStep = (SpreadVaultZoneStep)step;
                            SpawnRangeList<MobSpawn> spawnList = castZoneStep.Mobs;
                            // Make a data entry for each enemy
                            for (int spawnIndex = 0; spawnIndex < spawnList.Count; spawnIndex++)
                            {
                                // Get current enemy data
                                MobSpawn currentMob = spawnList.GetSpawn(spawnIndex);

                                IntRange floorRange = spawnList.GetSpawnRange(spawnIndex);
                                DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, null, floorRange.Min, floorRange.Max, null, isBasementFloor);
                                vaultSpawnList.Add(encounterData);
                            }
                        }
                    }

                    // Look through floor gen steps
                    if (currentSegment is LayeredSegment)
                    {
                        LayeredSegment currentLayeredSegment = (LayeredSegment)currentSegment;
                        List<IFloorGen> floorGenList = currentLayeredSegment.Floors;

                        for (int floorGenIndex = 0; floorGenIndex < floorGenList.Count; floorGenIndex++)
                        {
                            //Console.WriteLine(floorGenList[floorGenIndex].GetType());
                            if (floorGenList[floorGenIndex] is GridFloorGen)
                            {
                                PriorityList<GenStep<MapGenContext>> genStepList = new PriorityList<GenStep<MapGenContext>>();
                                GridFloorGen currentFloorGen = (GridFloorGen)floorGenList[floorGenIndex];
                                genStepList = currentFloorGen.GenSteps;

                                IEnumerable<Priority> genStepListOfPriorities = genStepList.GetPriorities();

                                foreach (Priority currentPriority in genStepListOfPriorities)
                                {
                                    IEnumerable<GenStep<MapGenContext>> genStepsAtCurrentPriority = genStepList.GetItems(currentPriority);

                                    foreach (GenStep<MapGenContext> currentGenStep in genStepsAtCurrentPriority)
                                    {
                                        //Console.WriteLine(currentGenStep.GetType().GetFormattedTypeName());

                                        // Check for connected room with guard
                                        if (currentGenStep is GuardSealStep<MapGenContext>)
                                        {
                                            GuardSealStep<MapGenContext> currentGuardSealGenStep = (GuardSealStep<MapGenContext>)currentGenStep;
                                            LoopedRand<MobSpawn> guardSpawnPicker = (LoopedRand<MobSpawn>)currentGuardSealGenStep.Guards;
                                            SpawnList<MobSpawn> guardSpawnList = (SpawnList<MobSpawn>)guardSpawnPicker.Spawner;

                                            for (int guardSpawnIndex = 0; guardSpawnIndex < guardSpawnList.Count; guardSpawnIndex++)
                                            {
                                                MobSpawn currentMob = guardSpawnList.GetSpawn(guardSpawnIndex);
                                                DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, null, floorGenIndex, floorGenIndex + 1, ["Spawns once, guarding secret stairs"], isBasementFloor);
                                                specialSpawnList.Add(encounterData);
                                            }
                                        }

                                        // Check for random mobs being placed
                                        if (currentGenStep is PlaceRandomMobsStep<MapGenContext>)
                                        {
                                            PlaceRandomMobsStep<MapGenContext> currentPlaceRandomMobsGenStep = (PlaceRandomMobsStep<MapGenContext>)currentGenStep;
                                            if (currentPlaceRandomMobsGenStep.Spawn.GetType() == typeof(LoopedTeamSpawner<MapGenContext>))
                                            {
                                                LoopedTeamSpawner<MapGenContext> currentTeamSpawner = (LoopedTeamSpawner<MapGenContext>)currentPlaceRandomMobsGenStep.Spawn;
                                                //Console.WriteLine(currentTeamSpawner.Picker.GetType().GetFormattedTypeName());
                                                if (currentTeamSpawner.Picker.GetType() == typeof(SpecificTeamSpawner))
                                                {
                                                    SpawnList<MobSpawn> potentialSpawnList = currentTeamSpawner.Picker.GetPossibleSpawns();
                                                    if (currentTeamSpawner.AmountSpawner.GetType() == typeof(RandDecay))
                                                    {
                                                        RandDecay currentRandDecaySpawner = (RandDecay)currentTeamSpawner.AmountSpawner;
                                                        for (int potentialSpawnIndex = 0; potentialSpawnIndex < potentialSpawnList.Count; potentialSpawnIndex++)
                                                        {
                                                            MobSpawn currentMob = potentialSpawnList.GetSpawn(potentialSpawnIndex);
                                                            DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, null, floorGenIndex, floorGenIndex + 1, [String.Format("Spawns {0}-{1} times per floor with a {2}% chance<br>Doesn't respawn", currentRandDecaySpawner.Min, currentRandDecaySpawner.Max, currentRandDecaySpawner.Rate)], isBasementFloor);
                                                            specialSpawnList.Add(encounterData);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            if (floorGenList[floorGenIndex] is RoomFloorGen)
                            {
                                PriorityList<GenStep<ListMapGenContext>> genStepList = new PriorityList<GenStep<ListMapGenContext>>();
                                RoomFloorGen currentFloorGen = (RoomFloorGen)floorGenList[floorGenIndex];
                                genStepList = currentFloorGen.GenSteps;
                                IEnumerable<Priority> genStepListOfPriorities = genStepList.GetPriorities();

                                foreach (Priority currentPriority in genStepListOfPriorities)
                                {
                                    IEnumerable<GenStep<ListMapGenContext>> genStepsAtCurrentPriority = genStepList.GetItems(currentPriority);

                                    foreach (GenStep<ListMapGenContext> currentGenStep in genStepsAtCurrentPriority)
                                    {
                                        // Check for terrain mobs
                                        if (currentGenStep is PlaceTerrainMobsStep<ListMapGenContext>)
                                        {
                                            //Console.WriteLine(currentGenStep.GetType().GetFormattedTypeName());

                                            PlaceTerrainMobsStep<ListMapGenContext> currentPlaceTerrainMobsGenStep = (PlaceTerrainMobsStep<ListMapGenContext>)currentGenStep;
                                            LoopedTeamSpawner<ListMapGenContext> terrainMobsSpawner = (LoopedTeamSpawner<ListMapGenContext>)currentPlaceTerrainMobsGenStep.Spawn;
                                            TeamSpawner specificSpawner = terrainMobsSpawner.Picker;
                                            if (specificSpawner.GetType() == typeof(PoolTeamSpawner))
                                            {
                                                PoolTeamSpawner specificPoolSpawner = (PoolTeamSpawner)specificSpawner;
                                                SpawnList<TeamMemberSpawn> terrainSpawns = specificPoolSpawner.Spawns;
                                                for (int currentSpawnIndex = 0; currentSpawnIndex < terrainSpawns.Count; currentSpawnIndex++)
                                                {
                                                    TeamMemberSpawn currentSpawn = terrainSpawns.GetSpawn(currentSpawnIndex);
                                                    MobSpawn currentMob = currentSpawn.Spawn;

                                                    DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, currentSpawn, floorGenIndex, floorGenIndex + 1, ["Spawns in tall grass"], isBasementFloor);
                                                    specialSpawnList.Add(encounterData);
                                                }
                                            }

                                        }

                                    }
                                }
                            }

                            if (floorGenList[floorGenIndex] is LoadGen)
                            {
                                PriorityList<GenStep<MapLoadContext>> genStepList = new PriorityList<GenStep<MapLoadContext>>();
                                LoadGen currentFloorGen = (LoadGen)floorGenList[floorGenIndex];
                                genStepList = currentFloorGen.GenSteps;

                                IEnumerable<Priority> genStepListOfPriorities = genStepList.GetPriorities();

                                foreach (Priority currentPriority in genStepListOfPriorities)
                                {
                                    IEnumerable<GenStep<MapLoadContext>> genStepsAtCurrentPriority = genStepList.GetItems(currentPriority);

                                    foreach (GenStep<MapLoadContext> currentGenStep in genStepsAtCurrentPriority)
                                    {
                                        if (currentGenStep is MappedRoomStep<MapLoadContext>)
                                        {
                                            // Get the ground map loaded by this load gen
                                            MappedRoomStep<MapLoadContext> currentMappedRoomGenStep = (MappedRoomStep<MapLoadContext>)currentGenStep;
                                            string mapID = currentMappedRoomGenStep.MapID;
                                            Map currentMap = DataManager.Instance.GetMap(mapID);

                                            if (zoneName == "")
                                            {
                                                zoneName = currentMap.Name.ToLocal();
                                                trimmedZoneName = zoneName.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("B{0}F", "").Replace("{0}F", "").TrimEnd().Replace(" ", "_");
                                                //Console.WriteLine(trimmedZoneName);
                                            }

                                            if (currentMap.MapTeams.Count > 0)
                                            {
                                                Team mapMobs = currentMap.MapTeams[0];
                                                foreach (Character currentMob in mapMobs.Players)
                                                {
                                                    //Console.WriteLine(currentMob.Name);

                                                    StaticSpawnData currentStaticSpawn = new StaticSpawnData();
                                                    currentStaticSpawn.spawnName = currentMob.Name;
                                                    currentStaticSpawn.level = currentMob.Level;
                                                    currentStaticSpawn.gender = (int)currentMob.CurrentForm.Gender;

                                                    foreach (SlotSkill currentSkill in currentMob.BaseSkills)
                                                    {
                                                        SkillData learnedSkill = DataManager.Instance.GetSkill(currentSkill.SkillNum);
                                                        currentStaticSpawn.specifiedSkillsList.Add("[[" + learnedSkill.Name.ToLocal() + "]]");
                                                    }
                                                    
                                                    currentStaticSpawn.spawnIntrinsic = DataManager.Instance.GetIntrinsic(currentMob.BaseIntrinsics[0]).Name.ToLocal();

                                                    currentStaticSpawn.extraFeatures.Add("Max HP: " + currentMob.MaxHP.ToString());
                                                    currentStaticSpawn.extraFeatures.Add("Attack: " + currentMob.Atk.ToString());
                                                    currentStaticSpawn.extraFeatures.Add("Defense: " + currentMob.Def.ToString());
                                                    currentStaticSpawn.extraFeatures.Add("Sp. Atk: " + currentMob.MAtk.ToString());
                                                    currentStaticSpawn.extraFeatures.Add("Sp. Def: " + currentMob.MDef.ToString());
                                                    currentStaticSpawn.extraFeatures.Add("Speed: " + currentMob.Speed.ToString());

                                                    if (currentMob.EquippedItem.ID != "")
                                                    {
                                                        ItemData mobHeldItem = DataManager.Instance.GetItem(currentMob.EquippedItem.ID);
                                                        currentStaticSpawn.extraFeatures.Add("Held: [[" + mobHeldItem.Name.ToLocal() + "]]");
                                                    }


                                                    staticSpawnList.Add(currentStaticSpawn);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (currentSegment is SingularSegment)
                    {
                        SingularSegment currentSingularSegment = (SingularSegment)currentSegment;
                        if (currentSingularSegment.BaseFloor.GetType() == typeof(LoadGen))
                        {
                            PriorityList<GenStep<MapLoadContext>> genStepList = new PriorityList<GenStep<MapLoadContext>>();
                            LoadGen currentFloorGen = (LoadGen)currentSingularSegment.BaseFloor;
                            genStepList = currentFloorGen.GenSteps;

                            IEnumerable<Priority> genStepListOfPriorities = genStepList.GetPriorities();

                            foreach (Priority currentPriority in genStepListOfPriorities)
                            {
                                IEnumerable<GenStep<MapLoadContext>> genStepsAtCurrentPriority = genStepList.GetItems(currentPriority);

                                foreach (GenStep<MapLoadContext> currentGenStep in genStepsAtCurrentPriority)
                                {
                                    // Get name of segment
                                    if (currentGenStep is MapNameIDStep<MapLoadContext>)
                                    {
                                        MapNameIDStep<MapLoadContext> currentMapNameIDGenStep = (MapNameIDStep<MapLoadContext>)currentGenStep;
                                        zoneName = mainZoneName + " " + currentMapNameIDGenStep.Name.DefaultText;
                                        trimmedZoneName = zoneName.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("B{0}F", "").Replace("{0}F", "").TrimEnd().Replace(" ", "_");
                                        //Console.WriteLine(trimmedZoneName);
                                    }

                                    // Check for mob spawns
                                    if (currentGenStep is PlaceTerrainMobsStep<MapLoadContext>)
                                    {
                                        PlaceTerrainMobsStep<MapLoadContext> currentPlaceTerrainMobsGenStep = (PlaceTerrainMobsStep<MapLoadContext>)currentGenStep;
                                        if (currentPlaceTerrainMobsGenStep.Spawn.GetType() == typeof(LoopedTeamSpawner<MapLoadContext>))
                                        {
                                            LoopedTeamSpawner<MapLoadContext> currentTeamSpawner = (LoopedTeamSpawner<MapLoadContext>)currentPlaceTerrainMobsGenStep.Spawn;
                                            if (currentTeamSpawner.Picker.GetType() == typeof(PoolTeamSpawner))
                                            {
                                                PoolTeamSpawner specificPoolSpawner = (PoolTeamSpawner)currentTeamSpawner.Picker;
                                                SpawnList<TeamMemberSpawn> terrainSpawns = specificPoolSpawner.Spawns;
                                                for (int currentSpawnIndex = 0; currentSpawnIndex < terrainSpawns.Count; currentSpawnIndex++)
                                                {
                                                    TeamMemberSpawn currentSpawn = terrainSpawns.GetSpawn(currentSpawnIndex);
                                                    MobSpawn currentMob = currentSpawn.Spawn;

                                                    DungeonSpawnData encounterData = GetDungeonEncounterData(currentMob, currentSpawn, 0, 1, ["Does not respawn"], isBasementFloor);
                                                    specialSpawnList.Add(encounterData);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Remove duplicate entries in special spawn list
                    for (int i = 0; i < specialSpawnList.Count; i++)
                    {
                        DungeonSpawnData currentEntry = specialSpawnList[i];
                        if (!currentEntry.isDuplicate)
                        {
                            // Check through the list to find duplicates
                            for (int j = i + 1; j < specialSpawnList.Count; j++)
                            {
                                DungeonSpawnData evaluatedEntry = specialSpawnList[j];

                                if (currentEntry.Equals(evaluatedEntry))
                                {
                                    currentEntry.endFloor = evaluatedEntry.endFloor;
                                    evaluatedEntry.isDuplicate = true;

                                    specialSpawnList.RemoveAt(i);
                                    specialSpawnList.Insert(i, currentEntry);

                                    specialSpawnList.RemoveAt(j);
                                    specialSpawnList.Insert(j, evaluatedEntry);
                                }
                            }
                        }
                    }
                    for(int i = specialSpawnList.Count - 1; i >= 0; i--)
                    {
                        if (specialSpawnList[i].isDuplicate)
                        {
                            specialSpawnList.RemoveAt(i);
                        }
                    }

                    // Sort spawn lists by start and end floor, then min and max level
                    segmentSpawnList.Sort(delegate (DungeonSpawnData x, DungeonSpawnData y)
                    {
                        if (x.startFloor != y.startFloor)
                        {
                            return x.startFloor.CompareTo(y.startFloor);
                        }
                        else if (x.endFloor != y.endFloor)
                        {
                            return x.endFloor.CompareTo(y.endFloor);
                        }
                        else if (x.minLevel != y.minLevel)
                        {
                            return x.minLevel.CompareTo(y.minLevel);
                        }
                        else
                        {
                            return x.maxLevel.CompareTo(y.maxLevel);
                        }
                    }
                    );
                    specialSpawnList.Sort(delegate (DungeonSpawnData x, DungeonSpawnData y)
                    {
                        if (x.startFloor != y.startFloor)
                        {
                            return x.startFloor.CompareTo(y.startFloor);
                        }
                        else if (x.endFloor != y.endFloor)
                        {
                            return x.endFloor.CompareTo(y.endFloor);
                        }
                        else if (x.minLevel != y.minLevel)
                        {
                            return x.minLevel.CompareTo(y.minLevel);
                        }
                        else
                        {
                            return x.maxLevel.CompareTo(y.maxLevel);
                        }
                    }
                    );

                    string fileContent = "";

                    // Output the spawn list
                    if (segmentSpawnList.Count > 0)
                    {
                        if (specialSpawnList.Count > 0 || vaultSpawnList.Count > 0)
                        {
                            fileContent += "=== Regular spawns ===\r\n\r\n{| class=\"wikitable\"\r\n{{EncounterHeader}}\r\n";
                        }
                        else
                        {
                            fileContent += "{| class=\"wikitable\"\r\n{{EncounterHeader}}\r\n";
                        }
                        foreach (DungeonSpawnData encounterData in segmentSpawnList)
                        {
                            fileContent += encounterData.ToString();
                        }
                        fileContent += "|}\r\n";
                    }

                    // Output the special spawn list
                    if (specialSpawnList.Count > 0)
                    {
                        fileContent += "\r\n=== Special spawns ===\r\n\r\n{| class=\"wikitable\"\r\n{{EncounterHeader}}\r\n";
                        foreach (DungeonSpawnData encounterData in specialSpawnList)
                        {
                            fileContent += encounterData.ToString();
                        }
                        fileContent += "|}\r\n";
                    }

                    // Output the vault spawn list
                    if (vaultSpawnList.Count > 0)
                    {
                        fileContent += "\r\n=== Vault spawns ===\r\n\r\n{| class=\"wikitable\"\r\n{{EncounterHeader}}\r\n";
                        foreach (DungeonSpawnData encounterData in vaultSpawnList)
                        {
                            fileContent += encounterData.ToString();
                        }
                        fileContent += "|}\r\n";
                    }

                    // Output the static spawn list
                    if (staticSpawnList.Count > 0)
                    {
                        fileContent += "\r\n=== Static spawns ===\r\n\r\n{| class=\"wikitable\"\r\n{{EncounterHeader}}\r\n";
                        foreach (StaticSpawnData encounterData in staticSpawnList)
                        {
                            fileContent += encounterData.ToString();
                        }
                        fileContent += "|}\r\n";
                    }

                    if (fileContent.Length > 0)
                    { 
                        string fileName = zoneName.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("B{0}F", "").Replace("{0}F", "").TrimEnd().Replace(" ", "_");
                        bool completed = WriteToWiki(fileName + "/Encounters", fileContent);
                        if (!completed)
                        {
                            conflictSegment++;
                            completed = WriteToWiki(fileName + "_" + conflictSegment.ToString() + "/Encounters", fileContent);
                        }
                    }
                }
            }
        }

        public struct DungeonSpawnData()
        {
            public string spawnName;

            public int minLevel;
            public int maxLevel;
            public bool levelCanBeIncreased;

            public int startFloor;
            public int endFloor;
            public bool isBasement;

            public string spawnIntrinsic;

            public List<string> specifiedSkillsList = new List<string>();

            public List<string> extraFeatures = new List<string>();

            public bool isDuplicate = false;

            public void setDuplicate(bool isDupe)
            {
                isDuplicate = isDupe;
            }

            public bool Equals(DungeonSpawnData otherEntry)
            {
                bool answer = false;

                if (spawnName.Equals(otherEntry.spawnName))
                {
                    if (minLevel == otherEntry.minLevel && maxLevel == otherEntry.maxLevel)
                    {
                        if (spawnIntrinsic.Equals(otherEntry.spawnIntrinsic))
                        {
                            if (specifiedSkillsList.Except(otherEntry.specifiedSkillsList).Count() == 0)
                            {
                                if (extraFeatures.Except(otherEntry.extraFeatures).Count() == 0)
                                {
                                    answer = true;
                                }
                                answer = true;
                            }
                        }
                    }
                }

                return answer;
            }

            public override string ToString()
            {
                string encounterRow = "{{EncounterRow";

                // Spawn name step
                encounterRow += "\r\n|pokemon=" + spawnName;

                // Level range step
                string levelRangeString;
                if (minLevel == maxLevel)
                {
                    levelRangeString = minLevel.ToString();
                }
                else
                {
                    levelRangeString = minLevel.ToString() + "-" + maxLevel.ToString();
                }
                encounterRow += "\r\n|level=" + levelRangeString;
                if (levelCanBeIncreased)
                {
                    encounterRow += "+";
                }

                // Floor range step
                encounterRow += "\r\n|start_floor=" + startFloor;
                encounterRow += "\r\n|end_floor=" + endFloor;
                if (isBasement)
                {
                    encounterRow += "\r\n|is_basement=true";
                }

                // Ability step
                if (spawnIntrinsic != "")
                {
                    encounterRow += "\r\n|ability=" + spawnIntrinsic;
                }

                // Moves step
                if (specifiedSkillsList.Count > 0)
                {
                    string specifiedSkillsString = "";
                    for(int i = 0; i < specifiedSkillsList.Count; i++)
                    {
                        specifiedSkillsString += specifiedSkillsList[i];
                        if (i <  specifiedSkillsList.Count - 1)
                        {
                            specifiedSkillsString += "<br>";
                        }
                    }
                    encounterRow += "\r\n|moves=" + specifiedSkillsString;
                }

                // Notes step
                if (extraFeatures.Count > 0)
                {
                    string notes = "\r\n|notes=";

                    for (int i = 0; i < extraFeatures.Count; i++)
                    {
                        notes += extraFeatures[i];
                        if (i < extraFeatures.Count - 1)
                        {
                            notes += "<br>";
                        }
                    }

                    encounterRow += notes;
                }

                // Footer step
                encounterRow += "\r\n}}\r\n";

                return encounterRow;
            }
        }

        public struct StaticSpawnData()
        {
            public string spawnName;

            public int level;

            public int gender = -1;
            private static string[] genderStrings = ["Genderless", "Male", "Female"];

            public string spawnIntrinsic;

            public List<string> specifiedSkillsList = new List<string>();

            public List<string> extraFeatures = new List<string>();

            public bool isDuplicate = false;

            public void setDuplicate(bool isDupe)
            {
                isDuplicate = isDupe;
            }

            public bool Equals(StaticSpawnData otherEntry)
            {
                bool answer = false;

                if (spawnName.Equals(otherEntry.spawnName))
                {
                    if (level == otherEntry.level)
                    {
                        if (gender == otherEntry.gender)
                        {
                            if (spawnIntrinsic.Equals(otherEntry.spawnIntrinsic))
                            {
                                if (specifiedSkillsList.Except(otherEntry.specifiedSkillsList).Count() == 0)
                                {
                                    if (extraFeatures.Except(otherEntry.extraFeatures).Count() == 0)
                                    {
                                        answer = true;
                                    }
                                    answer = true;
                                }
                            }
                        }
                    }
                }

                return answer;
            }

            public override string ToString()
            {
                string encounterRow = "{{EncounterRow";

                // Spawn name step
                encounterRow += "\r\n|pokemon=" + spawnName;

                // Level range step
                encounterRow += "\r\n|level=" + level.ToString();

                // Gender step
                if (gender != -1)
                {
                    encounterRow += "\r\n|gender=" + genderStrings[gender];
                }

                // Ability step
                if (spawnIntrinsic != "")
                {
                    encounterRow += "\r\n|ability=" + spawnIntrinsic;
                }

                // Moves step
                if (specifiedSkillsList.Count > 0)
                {
                    string specifiedSkillsString = "";
                    for (int i = 0; i < specifiedSkillsList.Count; i++)
                    {
                        specifiedSkillsString += specifiedSkillsList[i];
                        if (i < specifiedSkillsList.Count - 1)
                        {
                            specifiedSkillsString += "<br>";
                        }
                    }
                    encounterRow += "\r\n|moves=" + specifiedSkillsString;
                }

                // Notes step
                if (extraFeatures.Count > 0)
                {
                    string notes = "\r\n|notes=";

                    for (int i = 0; i < extraFeatures.Count; i++)
                    {
                        notes += extraFeatures[i];
                        if (i < extraFeatures.Count - 1)
                        {
                            notes += "<br>";
                        }
                    }

                    encounterRow += notes;
                }

                // Footer step
                encounterRow += "\r\n}}\r\n";

                return encounterRow;
            }
        }


        public static DungeonSpawnData GetDungeonEncounterData(MobSpawn currentMob, TeamMemberSpawn currentSpawn = null, int minFloor = 0, int maxFloor = 0, string[] addedFeatures = null, bool isBasementFloor = false)
        {
            DungeonSpawnData currentSpawnData = new DungeonSpawnData();

            // Form name step
            MonsterData currentEnemyData = DataManager.Instance.GetMonster(currentMob.BaseForm.Species);
            currentSpawnData.spawnName = currentEnemyData.Forms[currentMob.BaseForm.Form].FormName.ToLocal();

            // Level range step
            RandRange levelRange = currentMob.Level;
            currentSpawnData.minLevel = levelRange.Min;
            currentSpawnData.maxLevel = levelRange.Max;

            // Floor range step
            IntRange floorRange = new IntRange(minFloor, maxFloor);
            currentSpawnData.startFloor = floorRange.Min + 1;
            currentSpawnData.endFloor = floorRange.Max;
            currentSpawnData.isBasement = isBasementFloor;

            // Intrinsic step
            if (currentMob.Intrinsic != "")
            {
                IntrinsicData currentIntrinsicData = DataManager.Instance.GetIntrinsic(currentMob.Intrinsic);
                currentSpawnData.spawnIntrinsic = currentIntrinsicData.Name.ToLocal();
            }
            else
            {
                currentSpawnData.spawnIntrinsic = "";
            }

            // Specified skills list
            List<string> specifiedSkillsList = currentMob.SpecifiedSkills;
            for (int specifiedSkillIndex = 0; specifiedSkillIndex < specifiedSkillsList.Count; specifiedSkillIndex++)
            {
                string skill = specifiedSkillsList[specifiedSkillIndex];
                SkillData currentSkillData = DataManager.Instance.GetSkill(skill);
                currentSpawnData.specifiedSkillsList.Add("[[" + currentSkillData.Name.ToLocal() + "]]");
            }

            // Other qualities list
            if (addedFeatures != null)
            {
                for (int i = 0; i < addedFeatures.Length; i++)
                {
                    currentSpawnData.extraFeatures.Add(addedFeatures[i]);
                }
            }

            if (currentMob.Tactic == "wait_attack" || currentMob.Tactic == "turret")
            {
                currentSpawnData.extraFeatures.Add("Doesn't move");
            }
            for (int spawnFeatureIndex = 0; spawnFeatureIndex < currentMob.SpawnFeatures.Count; spawnFeatureIndex++)
            {
                MobSpawnExtra spawnFeature = currentMob.SpawnFeatures[spawnFeatureIndex];
                if (spawnFeature is MobSpawnItem)
                {
                    MobSpawnItem castFeature = (MobSpawnItem)spawnFeature;
                    ItemData heldItem = DataManager.Instance.GetItem(castFeature.Items.GetSpawn(0).ID);
                    currentSpawnData.extraFeatures.Add("Held: [[" + heldItem.Name.ToLocal() + "]]");
                }
                if (spawnFeature is MobSpawnStatus)
                {
                    MobSpawnStatus castFeature = (MobSpawnStatus)spawnFeature;
                    SpawnList<StatusEffect> statusList = castFeature.Statuses;
                    for(int statusIndex = 0; statusIndex < statusList.Count; statusIndex++)
                    {
                        StatusEffect currentStatus = statusList.GetSpawn(statusIndex);
                        if (currentStatus.ID == "sleep")
                        {
                            currentSpawnData.extraFeatures.Add("Spawns asleep");
                        }
                    }
                }
                if (spawnFeature is MobSpawnLevelScale)
                {
                    MobSpawnLevelScale castFeature = (MobSpawnLevelScale)spawnFeature;
                    string levelScaleString = "";
                    if (castFeature.StartFromID == 0)
                    {
                        levelScaleString = String.Format("Gains {0}/{1} levels every floor", castFeature.AddNumerator, castFeature.AddDenominator);
                    }
                    else
                    {
                        levelScaleString = String.Format("Starting at floor {0}, gains {1}/{2} levels every floor", castFeature.StartFromID + 1, castFeature.AddNumerator, castFeature.AddDenominator);
                    }
                    currentSpawnData.extraFeatures.Add(levelScaleString);
                    currentSpawnData.minLevel = castFeature.MinLevel;
                    currentSpawnData.maxLevel = castFeature.MinLevel;
                    currentSpawnData.levelCanBeIncreased = true;
                }
                /*
                if (spawnFeature is MobSpawnWeak)
                {
                    notes += "Half PP and 35% belly<br>";
                    noteCount++;
                }
                */
            }
            if (currentSpawn != null)
            {
                TeamMemberSpawn.MemberRole memberRole = currentSpawn.Role;
                if (memberRole == TeamMemberSpawn.MemberRole.Support)
                {
                    currentSpawnData.extraFeatures.Add("Spawns as team support");
                }
                if (memberRole == TeamMemberSpawn.MemberRole.Leader)
                {
                    currentSpawnData.extraFeatures.Add("Spawns as team leader");
                }
                if (memberRole == TeamMemberSpawn.MemberRole.Loner)
                {
                    currentSpawnData.extraFeatures.Add("Spawns alone");
                }
            }

            return currentSpawnData;
        }

        public static void ProgressBar(string message, string ending, int totalChunks, int progress, int total)
        {
#if !DEBUG
            // offset the progress by 1;
            progress++;
            Console.CursorVisible = false;

            double progressCompleted = (double)(progress) / total;
            int numChunksComplete = (int)(totalChunks * progressCompleted);
            string progressString = String.Format("Progress: [{0, 3}%]", (int)(progressCompleted * 100));
            string spacer = " ";

            int offset = progressString.Length + spacer.Length;

            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(progressString);
            
            Console.ResetColor();
            Console.Write(spacer);
            Console.CursorLeft = offset;
            Console.Write("[");
            Console.CursorLeft = offset + totalChunks + 1;
            Console.Write("|");
            Console.CursorLeft = offset + 1;
            
            if (numChunksComplete > 0 && progress != total)
                Console.Write("".PadRight(numChunksComplete - 1, '=') + ">");
            else
                Console.Write("".PadRight(numChunksComplete, '='));

            Console.Write("".PadRight(totalChunks - numChunksComplete, '-'));
            Console.CursorLeft = offset + totalChunks + 2;

            int totalDigits = total.ToString().Length;
            string display = String.Format(" {0}/{1} ]", progress.ToString().PadLeft(totalDigits), total);

 
            string currMessage;
            if (progress < total)
                currMessage = message;
            else
            {
                string spacePadding = new string(' ', Math.Abs(ending.Length - message.Length));
                currMessage = ending + spacePadding;
            }

            Console.Write(display + spacer + currMessage);
#endif
        }
    }
}
