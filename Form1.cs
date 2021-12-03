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
        private Dictionary<string, Nexomon> nexomonList;
        private Dictionary<int, Skill> skillList;
        private List<string> skillNames = new List<string>();
        private List<string> skillDescriptions = new List<string>();
        private Nexomon currentMon;
        private string currentDir = Environment.CurrentDirectory;



        // FORM LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
            // parse monsters and skills data
            //
            nexomonList = JsonConvert.DeserializeObject<Dictionary<string, Nexomon>>
                (File.ReadAllText($@"{currentDir}\Data\monsters.txt"));
            skillList = JsonConvert.DeserializeObject<Dictionary<int, Skill>>
                (File.ReadAllText($@"{currentDir}\Data\skills.txt"));


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


            // get skills names and descriptions
            //
            foreach (var value in skillList.Values)
            {
                skillNames.Add(value.name);
                skillDescriptions.Add(value.description);
            }

            // add skills to skills search list
            //
            foreach (string s in skillNames)
                listBox2.Items.Add(s);
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
            listBox2.SelectedIndex = 0;
        }



        // NEXOMON LIST SELECTED ITEM CHANGE
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get current mon
            //
            string monName = ConvertToLower(listBox1.SelectedItem.ToString());
            currentMon = nexomonList[monName];

            // set monster and element images
            //
            Bitmap bmpMon = (Bitmap)Properties.Resources.ResourceManager.GetObject(monName);
            pictureBox1.Image = bmpMon;
            Bitmap bmpType = (Bitmap)Properties.Resources.ResourceManager.GetObject(currentMon.element);
            pictureBox2.Image = bmpType;

            // set monster info texts
            //
            hpText.Text = $"HP:   {currentMon.hp}";
            staminaText.Text = $"STA: {currentMon.sta}";
            attackText.Text = $"ATK: {currentMon.atk}";
            defenseText.Text = $"DEF: {currentMon.def}";
            speedText.Text = $"SPD: {currentMon.spd}";
            descText.Text = currentMon.description;

            // clear old monster's skills and add the new monster's skills
            //
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


        // SKILLS SEARCH LIST SELECTED ITEM CHANGE
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get skill and skill index
            //
            var skill = skillList.Where(x => x.Value.name == listBox2.SelectedItem.ToString());
            var skillNum = skill.First().Key;

            // clear skillmons list and add new ones
            //
            listBox3.Items.Clear();
            foreach (var mon in nexomonList.Where(x => x.Value.skill_tree.Values.Contains(skillNum)))
            {
                var item = listBox3.Items.Add($"{ConvertToUpper(mon.Key)},  " +
                    $"Lv{mon.Value.skill_tree.Where(x => x.Value == skillNum).First().Key}");
            }

            // set first mon to be selected
            //
            listBox3.SelectedIndex = 0;
        }


        // SKILLS SEARCH NEXOMON CHANGED
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set current mon and image
            //
            string[] itemName = listBox3.SelectedItem.ToString().Split(',');
            string monName = ConvertToLower(itemName[0]);
            currentMon = nexomonList[monName];
            Bitmap bmpMon = (Bitmap)Properties.Resources.ResourceManager.GetObject(monName);
            pictureBox4.Image = bmpMon;
        }


        // SKILLS SEARCH NEXOMON VIEW CHAGE
        private void button1_Click(object sender, EventArgs e)
        {
            // change tab and selected nexomon
            //
            tabControl1.SelectedIndex = 0;
            listBox1.SelectedItem = ConvertToUpper(nexomonList.Where(x => x.Value == currentMon).First().Key);
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
