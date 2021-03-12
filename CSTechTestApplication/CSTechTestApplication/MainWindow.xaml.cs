using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSTechTestApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Url { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Baslangic();
        }

        private void Baslangic()
        {
            btn_DosyaSec.Content = "Dosya Seçiniz";
            brd_Komut.Visibility = Visibility.Collapsed;
            brd_Sonuc.Visibility = Visibility.Collapsed;
            brd_Mesaj.Visibility = Visibility.Collapsed;
            txt_Komut.Text = "";
            txt_Mesaj.Text = "";
            txt_FileName.Text = "";
        }

        private void btn_Calistir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Komut.Text))
            {
                MessageBox.Show("Komut giriniz!");
                return;
            }
            string input = txt_Komut.Text.Trim();
            int length = input.Length;


            List<string> inputVerbs = new List<string>();

            string verb = "";

            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrWhiteSpace(input[i].ToString()))
                {
                    verb = verb + input[i].ToString();

                    if (i == length - 1)
                        inputVerbs.Add(verb);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(verb))
                    {
                        inputVerbs.Add(verb);
                        verb = "";
                    }
                }
            }

            string firstverb = inputVerbs[0].ToUpper();

            string hataMesaji = KomutKontrol(firstverb, inputVerbs);
            if (!string.IsNullOrEmpty(hataMesaji))
            {
                MessageBox.Show(hataMesaji);
                brd_Mesaj.Visibility = Visibility.Collapsed;
                return;
            }

            if (firstverb == "F") //Find              
                FindVerbCount(inputVerbs);
            else if (firstverb == "R") // Replace            
                ReplaceVerb(inputVerbs[1], inputVerbs[2]);
            else// Delete           
                DeleteVerb(inputVerbs[1]);
        }

        private void FindVerbCount(List<string> inputVerbs)
        {
            List<string> listIcerikVerbs = new List<string>();
            listIcerikVerbs = GetIcerikVerbs();

            int count = 0;
            string sbString;

            brd_Mesaj.Visibility = Visibility.Visible;

            if (inputVerbs[1].Any(p => p.ToString().Contains("-"))) //-ata- benzeri aramalar için
            {
                sbString = inputVerbs[1].Substring(1, inputVerbs[1].Length - 2);

                count = listIcerikVerbs.Where(p => p.Length == inputVerbs[1].Length && p.Substring(1, inputVerbs[1].Length - 2) == sbString.ToLower()).Count();

                if (count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren " + inputVerbs[1].Length.ToString() + " harfli " + count.ToString() + " adet kelime mevcuttur";
                else
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren " + inputVerbs[1].Length.ToString() + " harfli kelime bulunmamaktadır";
            }
            else if (inputVerbs[1].Any(p => p.ToString().Contains("*"))) //*aba* benzeri aramalar için
            {

                sbString = inputVerbs[1].Substring(1, inputVerbs[1].Length - 2);

                count = listIcerikVerbs.Where(p => p.Contains(sbString.ToLower())).Count();

                if (count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren " + count.ToString() + " adet kelime mevcuttur";
                else
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren  kelime bulunmamaktadır";
            }
            else
            {
                count = listIcerikVerbs.Where(p => p == inputVerbs[1]).Count();

                if (count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + count.ToString() + " adet " + inputVerbs[1] + " kelimesi mevcuttur";
                else
                    txt_Mesaj.Text = " Dosyanızda " + inputVerbs[1] + " kelimesi bulunmamaktadır";
            }
        }

        private void ReplaceVerb(string oldword, string newword)
        {
            string fileIcerik = File.ReadAllText(Url);

            List<string> listIcerikVerbs = new List<string>();
            listIcerikVerbs = GetIcerikVerbs();

            List<string> replacedwords = new List<string>();

            brd_Mesaj.Visibility = Visibility.Visible;

            string sbString = "";
            if (oldword.Any(p => p.ToString().Contains("-"))) //-ata- benzeri aramalar için
            {
                sbString = oldword.Substring(1, oldword.Length - 2);

                replacedwords = listIcerikVerbs.Where(p => p.Length == oldword.Length && p.Substring(1, oldword.Length - 2) == sbString).Distinct().ToList();

                if (replacedwords.Count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren kelimeler " + newword + " olarak değiştirilmiştir";
                else
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren " + oldword.Length + " harfli  kelime bulunmamaktadır";


            }
            else if (oldword.Any(p => p.ToString().Contains("*"))) //*aba* benzeri aramalar için
            {
                sbString = oldword.Substring(1, oldword.Length - 2);

                replacedwords = listIcerikVerbs.Where(p => p.Contains(sbString)).Distinct().ToList();

                if (replacedwords.Count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren kelimeler " + newword + " olarak değiştirilmiştir";
                else
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren  kelime bulunmamaktadır";

            }
            else
            {
                replacedwords = listIcerikVerbs.Where(p => p == oldword).Distinct().ToList();

                if (replacedwords.Count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + oldword + " kelimeleri  " + newword + " olarak değiştirilmiştir";
                else
                    txt_Mesaj.Text = " Dosyanızda " + oldword + " kelimelesi bulunmamaktadır";

            }

            foreach (string item in replacedwords)
                fileIcerik = fileIcerik.Replace(item, newword);


            File.WriteAllText(Url, fileIcerik);
            txt_Icerik.Text = File.ReadAllText(Url);
        }

        private void DeleteVerb(string dword)
        {
            string fileIcerik = File.ReadAllText(Url);

            List<string> listIcerikVerbs = new List<string>();
            listIcerikVerbs = GetIcerikVerbs();

            List<string> deletedWords = new List<string>();

            brd_Mesaj.Visibility = Visibility.Visible;

            string sbString = "";
            if (dword.Any(p => p.ToString().Contains("-"))) //-ata- benzeri aramalar için
            {
                sbString = dword.Substring(1, dword.Length - 2);

                deletedWords = listIcerikVerbs.Where(p => p.Length == dword.Length && p.Substring(1, dword.Length - 2) == sbString).Distinct().ToList();

                if (deletedWords.Count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren kelimeler silinmiştir";
                else
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren " + dword.Length + " harfli  kelime bulunmamaktadır";


            }
            else if (dword.Any(p => p.ToString().Contains("*"))) //*aba* benzeri aramalar için
            {
                sbString = dword.Substring(1, dword.Length - 2);

                deletedWords = listIcerikVerbs.Where(p => p.Contains(sbString)).Distinct().ToList();

                if (deletedWords.Count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren kelimeler silinmiştir";
                else
                    txt_Mesaj.Text = " Dosyanızda " + sbString + " ifadesini içeren  kelime bulunmamaktadır";

            }
            else
            {
                deletedWords = listIcerikVerbs.Where(p => p == dword).Distinct().ToList();

                if (deletedWords.Count > 0)
                    txt_Mesaj.Text = " Dosyanızda " + dword + " kelimeleri  silinmiştir";
                else
                    txt_Mesaj.Text = " Dosyanızda " + dword + " kelimesibulunmamaktadır";

            }

            foreach (string item in deletedWords)
                fileIcerik = fileIcerik.Replace(item, "");


            File.WriteAllText(Url, fileIcerik);
            txt_Icerik.Text = File.ReadAllText(Url);

        }

        private List<string> GetIcerikVerbs()
        {
            List<string> listIcerikVerbs = new List<string>();

            string icerik = txt_Icerik.Text.Trim();
            int lengthicerik = txt_Icerik.Text.Trim().Length;
            string verb = "";
            for (int i = 0; i < lengthicerik; i++)
            {
                if (!string.IsNullOrWhiteSpace(icerik[i].ToString()))
                {
                    verb = verb + icerik[i].ToString();

                    if (i == lengthicerik - 1)
                        listIcerikVerbs.Add(verb);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(verb))
                    {
                        listIcerikVerbs.Add(verb);
                        verb = "";
                    }
                }
            }
            return listIcerikVerbs;
        }

        private string KomutKontrol(string firstverb, List<string> verbs)
        {
            if (firstverb == "F" || firstverb == "R" || firstverb == "D")
            {
                if (firstverb == "F" || firstverb == "D")
                {
                    if (verbs.Count != 2)
                        return "F ve D  komutları için komut dışında bir adet kelime olmalıdır !";

                }
                else
                {
                    if (verbs.Count != 3)
                        return "R komutu için R komutuyla beraber iki adet kelime olmalıdır !";
                }

                //  - and * kontrol

                if (verbs[1].Any(p => p.ToString().Contains("-")) || verbs[1].Any(p => p.ToString().Contains("*")))
                {
                    if (verbs[1].First() != verbs[1].Last())
                    {
                        return "- , * kullanımı doğru şekilde değildir";
                    }

                    string sbString = verbs[1].Substring(1, verbs[1].Length - 2);

                    if (sbString.Length==0)
                    {
                        return "- , * kullanımı doğru şekilde değildir";
                    }

                    if (sbString.Contains("-") || sbString.Contains("*"))
                    {
                        return "- , * kullanımı doğru şekilde değildir";
                    }
                }

                return "";
            }
            else
                return "Komut Satırınız hatalıdır ! ipuçları kısmında örnek kullanımlar mevcuttur.";

        }

        private void btn_DosyaSec_Click(object sender, RoutedEventArgs e)
        {
            if (btn_DosyaSec.Content.ToString() == "Dosya Seçiniz")
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text files (*.txt)|*.txt";
                if (ofd.ShowDialog() == true)
                {
                    btn_DosyaSec.Content = "Temizle";
                    txt_FileName.Text = ofd.SafeFileName;
                    brd_Komut.Visibility = brd_Sonuc.Visibility = Visibility.Visible;
                    txt_Icerik.Text = File.ReadAllText(ofd.FileName);
                    Url = ofd.FileName;
                }
            }
            else // Temizle
            {
                Baslangic();
            }

        }
    
    }
}
