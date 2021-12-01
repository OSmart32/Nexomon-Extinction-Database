using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Nexomon_Extinction_Database
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Nexomon> nexomonList = new Dictionary<string, Nexomon>();
        private List<string> skillNames, skillDescriptions;
        private Dictionary<int, Skill> skillList = new Dictionary<int, Skill>();
        private string currentDir = Environment.CurrentDirectory;
        private Nexomon currentMon;



        // FORM LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
            // parse monsters data
            //
            nexomonList = JsonConvert.DeserializeObject<Dictionary<string, Nexomon>>
                (File.ReadAllText($@"{currentDir}\Data\monsters.txt"));

            /* Add the nexomon names to the list
             * Evolutions do not have a skill tree, so check previous index for skill tree
            */
            foreach (string mon in nexomonList.Keys)
            {
                listBox1.Items.Add(ConvertToUpper(mon));

                if (nexomonList[mon].skill_tree.Count == 0)
                {
                    int monIndex = nexomonList.Keys.ToList().FindIndex(x => x == mon);
                    nexomonList[mon].skill_tree = nexomonList[nexomonList.Keys.ToList()[monIndex - 1]].skill_tree;
                }
            }
            

            // parse skills data
            //
            skillList = JsonConvert.DeserializeObject<Dictionary<int, Skill>>
                (File.ReadAllText($@"{currentDir}\Data\skills.txt"));


            skillNames = new List<string>();
            skillDescriptions = new List<string>();


            // parse skills names and descriptions
            //
            foreach (var value in skillList.Values)
            {
                skillNames.Add(value.name);
                skillDescriptions.Add(value.description);
            }
        }



        // Initialize
        public Form1()
        {
            InitializeComponent();
        }



        // FORM FIRST SHOWN
        private void Form1_Shown(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
        }



        // NEXOMON LIST SELECTED ITEM CHANGE
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string monName = ConvertToLower(listBox1.SelectedItem.ToString());
            currentMon = nexomonList[monName];

            Bitmap bmpMon = (Bitmap)Properties.Resources.ResourceManager.GetObject(monName);
            pictureBox1.Image = bmpMon;
            Bitmap bmpType = (Bitmap)Properties.Resources.ResourceManager.GetObject(currentMon.element);
            pictureBox2.Image = bmpType;

            hpText.Text = $"HP:   {currentMon.hp}";
            staminaText.Text = $"STA: {currentMon.sta}";
            attackText.Text = $"ATK: {currentMon.atk}";
            defenseText.Text = $"DEF: {currentMon.def}";
            speedText.Text = $"SPD: {currentMon.spd}";
            descText.Text = currentMon.description;

            listView1.Items.Clear();
            for (int i = 0; i < currentMon.skill_tree.Count; i++)
            {
                var skill = skillList[currentMon.skill_tree.Values.ToList()[i]];
                var keys = currentMon.skill_tree.Keys.ToList();
                var values = currentMon.skill_tree.Values.ToList();
                var move = listView1.Items.Add(skill.name, values[i] - 1);
                move.ToolTipText =
                    $"{skill.description}" +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"Power: {skill.damage_target_power}   " +
                    $"Accuracy: {skill.accuracy}%   " +
                    $"Sta: {skill.cost}   " +
                    $"Speed: {skill.speed}   " +
                    $"Crit: {skill.critical}%   " +
                    $"Lv{keys[i]}";
            }
        }



        // STRING CONVERT FOR LIST NAMES
        private string ConvertToUpper(string input)
        {
            StringBuilder sb = new StringBuilder();

            bool caseFlag = true;


            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];

                if (c == ' ')
                {
                    sb.Append(' ');
                    caseFlag = true;
                }
                else
                {
                    if (caseFlag)
                    {
                        sb.Append(char.ToUpper(c));
                        caseFlag = false;
                    }
                    else
                        sb.Append(char.ToLower(c));
                }
            }


            return sb.ToString();
        }


        // STRING CONVERT FOR LIST NAMES
        private string ConvertToLower(string input)
        {
            StringBuilder sb = new StringBuilder();

            bool caseFlag = false;


            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];

                if (c == ' ')
                {
                    sb.Append(' ');
                    caseFlag = true;
                }
                else
                {
                    if (caseFlag)
                    {
                        sb.Append(char.ToLower(c));
                        caseFlag = false;
                    }
                    else
                        sb.Append(char.ToLower(c));
                }
            }


            return sb.ToString();
        }
    }




    public class Nexomon
    {
        public string description { get; set; }
        public string element { get; set; }
        public int rarity { get; set; }
        public int evolves_at { get; set; }
        public int evolves_to { get; set; }
        public int hp { get; set; }
        public int sta { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int spd { get; set; }
        public string[] foods { get; set; }
        public Dictionary<int, int> skill_tree { get; set; }
    }


    public class Skill
    {
        public string name { get; set; }
        public int cost { get; set; }
        public int speed { get; set; }
        public string element { get; set; }
        public int damage_target_power { get; set; }
        public int accuracy { get; set; }
        public int critical { get; set; }
        public string description { get; set; }
    }
}
