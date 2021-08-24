#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;
using WinForm2048.Properties;

namespace WinForm2048
{
  public partial class FormMain : Form
  {
    public FormMain()
    {
      InitializeComponent();
    }

    internal readonly Dictionary<string, string> LanguageDicoEn = new Dictionary<string, string>();
    internal readonly Dictionary<string, string> LanguageDicoFr = new Dictionary<string, string>();
    private string _currentLanguage = "english";
    private ConfigurationOptions _configurationOptions = new ConfigurationOptions();
    private int[,] board = new int[9, 9];
    private int score = 0;
    private int highestScore = 0;

    private void QuitToolStripMenuItemClick(object sender, EventArgs e)
    {
      SaveWindowValue();
      Application.Exit();
    }

    private void AboutToolStripMenuItemClick(object sender, EventArgs e)
    {
      AboutBoxApplication aboutBoxApplication = new AboutBoxApplication();
      aboutBoxApplication.ShowDialog();
    }

    private void DisplayTitle()
    {
      Text += $" {GetVersion()}";
    }

    public static string GetVersion()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return $"V{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}.{fvi.FilePrivatePart}";
    }

    private void FormMainLoad(object sender, EventArgs e)
    {
      LoadSettingsAtStartup();
      DisableSwipeButtons();
    }

    private void DisableSwipeButtons()
    {
      buttonUp.Enabled = false;
      buttonDown.Enabled = false;
      buttonRight.Enabled = false;
      buttonLeft.Enabled = false;
    }

    private void LoadSettingsAtStartup()
    {
      DisplayTitle();
      GetWindowValue();
      LoadLanguages();
      SetLanguage(Settings.Default.LastLanguageUsed);
    }

    private void LoadConfigurationOptions()
    {
      _configurationOptions.Option1Name = Settings.Default.Option1Name;
      _configurationOptions.Option2Name = Settings.Default.Option2Name;
    }

    private void SaveConfigurationOptions()
    {
      // TODO adapt accordingly
      //_configurationOptions.Option1Name = Settings.Default.Option1Name;
      //_configurationOptions.Option2Name = Settings.Default.Option2Name;
    }

    private void LoadLanguages()
    {
      if (!File.Exists(Settings.Default.LanguageFileName))
      {
        CreateLanguageFile();
      }

      // read the translation file and feed the language
      XDocument xDoc;
      try
      {
        xDoc = XDocument.Load(Settings.Default.LanguageFileName);
      }
      catch (Exception exception)
      {
        MessageBox.Show(Resources.Error_while_loading_the + Punctuation.OneSpace +
          Settings.Default.LanguageFileName + Punctuation.OneSpace + Resources.XML_file +
          Punctuation.OneSpace + exception.Message);
        CreateLanguageFile();
        return;
      }
      var result = from node in xDoc.Descendants("term")
                   where node.HasElements
                   let xElementName = node.Element("name")
                   where xElementName != null
                   let xElementEnglish = node.Element("englishValue")
                   where xElementEnglish != null
                   let xElementFrench = node.Element("frenchValue")
                   where xElementFrench != null
                   select new
                   {
                     name = xElementName.Value,
                     englishValue = xElementEnglish.Value,
                     frenchValue = xElementFrench.Value
                   };
      foreach (var i in result)
      {
        if (!LanguageDicoEn.ContainsKey(i.name))
        {
          LanguageDicoEn.Add(i.name, i.englishValue);
        }
#if DEBUG
        else
        {
          MessageBox.Show(Resources.Your_XML_file_has_duplicate_like + Punctuation.Colon +
            Punctuation.OneSpace + i.name);
        }
#endif
        if (!LanguageDicoFr.ContainsKey(i.name))
        {
          LanguageDicoFr.Add(i.name, i.frenchValue);
        }
#if DEBUG
        else
        {
          MessageBox.Show(Resources.Your_XML_file_has_duplicate_like + Punctuation.Colon +
            Punctuation.OneSpace + i.name);
        }
#endif
      }
    }

    private static void CreateLanguageFile()
    {
      var minimumVersion = new List<string>
      {
        "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
        "<terms>",
         "<term>",
        "<name>MenuFile</name>",
        "<englishValue>File</englishValue>",
        "<frenchValue>Fichier</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileNew</name>",
        "<englishValue>New</englishValue>",
        "<frenchValue>Nouveau</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileOpen</name>",
        "<englishValue>Open</englishValue>",
        "<frenchValue>Ouvrir</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileSave</name>",
        "<englishValue>Save</englishValue>",
        "<frenchValue>Enregistrer</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFileSaveAs</name>",
        "<englishValue>Save as ...</englishValue>",
        "<frenchValue>Enregistrer sous ...</frenchValue>",
        "</term>",
        "<term>",
        "<name>MenuFilePrint</name>",
        "<englishValue>Print ...</englishValue>",
        "<frenchValue>Imprimer ...</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenufilePageSetup</name>",
          "<englishValue>Page setup</englishValue>",
          "<frenchValue>Aperçu avant impression</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenufileQuit</name>",
          "<englishValue>Quit</englishValue>",
          "<frenchValue>Quitter</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEdit</name>",
          "<englishValue>Edit</englishValue>",
          "<frenchValue>Edition</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditCancel</name>",
          "<englishValue>Cancel</englishValue>",
          "<frenchValue>Annuler</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditRedo</name>",
          "<englishValue>Redo</englishValue>",
          "<frenchValue>Rétablir</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditCut</name>",
          "<englishValue>Cut</englishValue>",
          "<frenchValue>Couper</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditCopy</name>",
          "<englishValue>Copy</englishValue>",
          "<frenchValue>Copier</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditPaste</name>",
          "<englishValue>Paste</englishValue>",
          "<frenchValue>Coller</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuEditSelectAll</name>",
          "<englishValue>Select All</englishValue>",
          "<frenchValue>Sélectionner tout</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuTools</name>",
          "<englishValue>Tools</englishValue>",
          "<frenchValue>Outils</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuToolsCustomize</name>",
          "<englishValue>Customize ...</englishValue>",
          "<frenchValue>Personaliser ...</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuToolsOptions</name>",
          "<englishValue>Options</englishValue>",
          "<frenchValue>Options</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuLanguage</name>",
          "<englishValue>Language</englishValue>",
          "<frenchValue>Langage</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuLanguageEnglish</name>",
          "<englishValue>English</englishValue>",
          "<frenchValue>Anglais</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuLanguageFrench</name>",
          "<englishValue>French</englishValue>",
          "<frenchValue>Français</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelp</name>",
          "<englishValue>Help</englishValue>",
          "<frenchValue>Aide</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpSummary</name>",
          "<englishValue>Summary</englishValue>",
          "<frenchValue>Sommaire</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpIndex</name>",
          "<englishValue>Index</englishValue>",
          "<frenchValue>Index</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpSearch</name>",
          "<englishValue>Search</englishValue>",
          "<frenchValue>Rechercher</frenchValue>",
        "</term>",
        "<term>",
          "<name>MenuHelpAbout</name>",
          "<englishValue>About</englishValue>",
          "<frenchValue>A propos de ...</frenchValue>",
        "</term>",
        "</terms>"
      };

      StreamWriter sw = new StreamWriter(Settings.Default.LanguageFileName);
      foreach (string item in minimumVersion)
      {
        sw.WriteLine(item);
      }

      sw.Close();
    }

    private void GetWindowValue()
    {
      Width = Settings.Default.WindowWidth;
      Height = Settings.Default.WindowHeight;
      Top = Settings.Default.WindowTop < 0 ? 0 : Settings.Default.WindowTop;
      Left = Settings.Default.WindowLeft < 0 ? 0 : Settings.Default.WindowLeft;
      labelScore.Text = $"SCORE: {Settings.Default.Score}";
      score = Settings.Default.Score;
      labelHighestScore.Text = $"HIGHEST SCORE: {Settings.Default.HighestScore}";
      highestScore = Settings.Default.HighestScore;
      SetDisplayOption(Settings.Default.DisplayToolStripMenuItem);
      LoadConfigurationOptions();
    }

    private void SaveWindowValue()
    {
      Settings.Default.WindowHeight = Height;
      Settings.Default.WindowWidth = Width;
      Settings.Default.WindowLeft = Left;
      Settings.Default.WindowTop = Top;
      Settings.Default.LastLanguageUsed = frenchToolStripMenuItem.Checked ? "French" : "English";
      Settings.Default.Score = int.Parse(labelScore.Text.Substring(7, labelScore.Text.Length - 7));
      int initialLength = "HIGHEST SCORE: ".Length;
      int length = labelHighestScore.Text.Length - "HIGHEST SCORE: ".Length;
      Settings.Default.HighestScore = int.Parse(labelHighestScore.Text.Substring(initialLength, length));
      Settings.Default.DisplayToolStripMenuItem = GetDisplayOption();
      SaveConfigurationOptions();
      Settings.Default.Save();
    }

    private string GetDisplayOption()
    {
      if (SmallToolStripMenuItem.Checked)
      {
        return "Small";
      }

      if (MediumToolStripMenuItem.Checked)
      {
        return "Medium";
      }

      return LargeToolStripMenuItem.Checked ? "Large" : string.Empty;
    }

    private void SetDisplayOption(string option)
    {
      UncheckAllOptions();
      switch (option.ToLower())
      {
        case "small":
          SmallToolStripMenuItem.Checked = true;
          break;
        case "medium":
          MediumToolStripMenuItem.Checked = true;
          break;
        case "large":
          LargeToolStripMenuItem.Checked = true;
          break;
        default:
          SmallToolStripMenuItem.Checked = true;
          break;
      }
    }

    private void UncheckAllOptions()
    {
      SmallToolStripMenuItem.Checked = false;
      MediumToolStripMenuItem.Checked = false;
      LargeToolStripMenuItem.Checked = false;
    }

    private void FormMainFormClosing(object sender, FormClosingEventArgs e)
    {
      SaveWindowValue();
    }

    private void FrenchToolStripMenuItemClick(object sender, EventArgs e)
    {
      _currentLanguage = Language.French.ToString();
      SetLanguage(Language.French.ToString());
      AdjustAllControls();
    }

    private void EnglishToolStripMenuItemClick(object sender, EventArgs e)
    {
      _currentLanguage = Language.English.ToString();
      SetLanguage(Language.English.ToString());
      AdjustAllControls();
    }

    private void SetLanguage(string myLanguage)
    {
      switch (myLanguage)
      {
        case "English":
          frenchToolStripMenuItem.Checked = false;
          englishToolStripMenuItem.Checked = true;
          fileToolStripMenuItem.Text = LanguageDicoEn["MenuFile"];
          newToolStripMenuItem.Text = LanguageDicoEn["MenuFileNew"];
          openToolStripMenuItem.Text = LanguageDicoEn["MenuFileOpen"];
          saveToolStripMenuItem.Text = LanguageDicoEn["MenuFileSave"];
          saveasToolStripMenuItem.Text = LanguageDicoEn["MenuFileSaveAs"];
          printPreviewToolStripMenuItem.Text = LanguageDicoEn["MenuFilePrint"];
          printPreviewToolStripMenuItem.Text = LanguageDicoEn["MenufilePageSetup"];
          quitToolStripMenuItem.Text = LanguageDicoEn["MenufileQuit"];
          editToolStripMenuItem.Text = LanguageDicoEn["MenuEdit"];
          cancelToolStripMenuItem.Text = LanguageDicoEn["MenuEditCancel"];
          redoToolStripMenuItem.Text = LanguageDicoEn["MenuEditRedo"];
          cutToolStripMenuItem.Text = LanguageDicoEn["MenuEditCut"];
          copyToolStripMenuItem.Text = LanguageDicoEn["MenuEditCopy"];
          pasteToolStripMenuItem.Text = LanguageDicoEn["MenuEditPaste"];
          selectAllToolStripMenuItem.Text = LanguageDicoEn["MenuEditSelectAll"];
          toolsToolStripMenuItem.Text = LanguageDicoEn["MenuTools"];
          personalizeToolStripMenuItem.Text = LanguageDicoEn["MenuToolsCustomize"];
          optionsToolStripMenuItem.Text = LanguageDicoEn["MenuToolsOptions"];
          languagetoolStripMenuItem.Text = LanguageDicoEn["MenuLanguage"];
          englishToolStripMenuItem.Text = LanguageDicoEn["MenuLanguageEnglish"];
          frenchToolStripMenuItem.Text = LanguageDicoEn["MenuLanguageFrench"];
          helpToolStripMenuItem.Text = LanguageDicoEn["MenuHelp"];
          summaryToolStripMenuItem.Text = LanguageDicoEn["MenuHelpSummary"];
          indexToolStripMenuItem.Text = LanguageDicoEn["MenuHelpIndex"];
          searchToolStripMenuItem.Text = LanguageDicoEn["MenuHelpSearch"];
          aboutToolStripMenuItem.Text = LanguageDicoEn["MenuHelpAbout"];
          DisplayToolStripMenuItem.Text = LanguageDicoEn["Display"];
          SmallToolStripMenuItem.Text = LanguageDicoEn["Small"];
          MediumToolStripMenuItem.Text = LanguageDicoEn["Medium"];
          LargeToolStripMenuItem.Text = LanguageDicoEn["Large"];


          _currentLanguage = "English";
          break;
        case "French":
          frenchToolStripMenuItem.Checked = true;
          englishToolStripMenuItem.Checked = false;
          fileToolStripMenuItem.Text = LanguageDicoFr["MenuFile"];
          newToolStripMenuItem.Text = LanguageDicoFr["MenuFileNew"];
          openToolStripMenuItem.Text = LanguageDicoFr["MenuFileOpen"];
          saveToolStripMenuItem.Text = LanguageDicoFr["MenuFileSave"];
          saveasToolStripMenuItem.Text = LanguageDicoFr["MenuFileSaveAs"];
          printPreviewToolStripMenuItem.Text = LanguageDicoFr["MenuFilePrint"];
          printPreviewToolStripMenuItem.Text = LanguageDicoFr["MenufilePageSetup"];
          quitToolStripMenuItem.Text = LanguageDicoFr["MenufileQuit"];
          editToolStripMenuItem.Text = LanguageDicoFr["MenuEdit"];
          cancelToolStripMenuItem.Text = LanguageDicoFr["MenuEditCancel"];
          redoToolStripMenuItem.Text = LanguageDicoFr["MenuEditRedo"];
          cutToolStripMenuItem.Text = LanguageDicoFr["MenuEditCut"];
          copyToolStripMenuItem.Text = LanguageDicoFr["MenuEditCopy"];
          pasteToolStripMenuItem.Text = LanguageDicoFr["MenuEditPaste"];
          selectAllToolStripMenuItem.Text = LanguageDicoFr["MenuEditSelectAll"];
          toolsToolStripMenuItem.Text = LanguageDicoFr["MenuTools"];
          personalizeToolStripMenuItem.Text = LanguageDicoFr["MenuToolsCustomize"];
          optionsToolStripMenuItem.Text = LanguageDicoFr["MenuToolsOptions"];
          languagetoolStripMenuItem.Text = LanguageDicoFr["MenuLanguage"];
          englishToolStripMenuItem.Text = LanguageDicoFr["MenuLanguageEnglish"];
          frenchToolStripMenuItem.Text = LanguageDicoFr["MenuLanguageFrench"];
          helpToolStripMenuItem.Text = LanguageDicoFr["MenuHelp"];
          summaryToolStripMenuItem.Text = LanguageDicoFr["MenuHelpSummary"];
          indexToolStripMenuItem.Text = LanguageDicoFr["MenuHelpIndex"];
          searchToolStripMenuItem.Text = LanguageDicoFr["MenuHelpSearch"];
          aboutToolStripMenuItem.Text = LanguageDicoFr["MenuHelpAbout"];
          DisplayToolStripMenuItem.Text = LanguageDicoFr["Display"];
          SmallToolStripMenuItem.Text = LanguageDicoFr["Small"];
          MediumToolStripMenuItem.Text = LanguageDicoFr["Medium"];
          LargeToolStripMenuItem.Text = LanguageDicoFr["Large"];

          _currentLanguage = "French";
          break;
        default:
          SetLanguage("English");
          break;
      }
    }

    private void CutToolStripMenuItemClick(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      var tb = focusedControl as TextBox;
      if (tb != null)
      {
        CutToClipboard(tb);
      }
    }

    private void CopyToolStripMenuItemClick(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      var tb = focusedControl as TextBox;
      if (tb != null)
      {
        CopyToClipboard(tb);
      }
    }

    private void PasteToolStripMenuItemClick(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      var tb = focusedControl as TextBox;
      if (tb != null)
      {
        PasteFromClipboard(tb);
      }
    }

    private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
    {
      Control focusedControl = FindFocusedControl(new List<Control> { }); // add your controls in the List
      TextBox control = focusedControl as TextBox;
      if (control != null) control.SelectAll();
    }

    private void CutToClipboard(TextBoxBase tb, string errorMessage = "nothing")
    {
      if (tb != ActiveControl) return;
      if (tb.Text == string.Empty)
      {
        DisplayMessage(Translate("ThereIs") + Punctuation.OneSpace +
          Translate(errorMessage) + Punctuation.OneSpace +
          Translate("ToCut") + Punctuation.OneSpace, Translate(errorMessage),
          MessageBoxButtons.OK);
        return;
      }

      if (tb.SelectedText == string.Empty)
      {
        DisplayMessage(Translate("NoTextHasBeenSelected"),
          Translate(errorMessage), MessageBoxButtons.OK);
        return;
      }

      Clipboard.SetText(tb.SelectedText);
      tb.SelectedText = string.Empty;
    }

    private void CopyToClipboard(TextBoxBase tb, string message = "nothing")
    {
      if (tb != ActiveControl) return;
      if (tb.Text == string.Empty)
      {
        DisplayMessage(Translate("ThereIsNothingToCopy") + Punctuation.OneSpace,
          Translate(message), MessageBoxButtons.OK);
        return;
      }

      if (tb.SelectedText == string.Empty)
      {
        DisplayMessage(Translate("NoTextHasBeenSelected"),
          Translate(message), MessageBoxButtons.OK);
        return;
      }

      Clipboard.SetText(tb.SelectedText);
    }

    private void PasteFromClipboard(TextBoxBase tb)
    {
      if (tb != ActiveControl) return;
      var selectionIndex = tb.SelectionStart;
      tb.SelectedText = Clipboard.GetText();
      tb.SelectionStart = selectionIndex + Clipboard.GetText().Length;
    }

    private void DisplayMessage(string message, string title, MessageBoxButtons buttons)
    {
      MessageBox.Show(this, message, title, buttons);
    }

    private string Translate(string index)
    {
      string result = string.Empty;
      switch (_currentLanguage.ToLower())
      {
        case "english":
          result = LanguageDicoEn.ContainsKey(index) ? LanguageDicoEn[index] :
           "the term: \"" + index + "\" has not been translated yet.\nPlease tell the developer to translate this term";
          break;
        case "french":
          result = LanguageDicoFr.ContainsKey(index) ? LanguageDicoFr[index] :
            "the term: \"" + index + "\" has not been translated yet.\nPlease tell the developer to translate this term";
          break;
      }

      return result;
    }

    private static Control FindFocusedControl(Control container)
    {
      foreach (Control childControl in container.Controls.Cast<Control>().Where(childControl => childControl.Focused))
      {
        return childControl;
      }

      return (from Control childControl in container.Controls
              select FindFocusedControl(childControl)).FirstOrDefault(maybeFocusedControl => maybeFocusedControl != null);
    }

    private static Control FindFocusedControl(List<Control> container)
    {
      return container.FirstOrDefault(control => control.Focused);
    }

    private static Control FindFocusedControl(params Control[] container)
    {
      return container.FirstOrDefault(control => control.Focused);
    }

    private static Control FindFocusedControl(IEnumerable<Control> container)
    {
      return container.FirstOrDefault(control => control.Focused);
    }

    private static string PeekDirectory()
    {
      string result = string.Empty;
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      if (fbd.ShowDialog() == DialogResult.OK)
      {
        result = fbd.SelectedPath;
      }

      return result;
    }

    private string PeekFile()
    {
      string result = string.Empty;
      OpenFileDialog fd = new OpenFileDialog();
      if (fd.ShowDialog() == DialogResult.OK)
      {
        result = fd.SafeFileName;
      }

      return result;
    }

    private void SmallToolStripMenuItemClick(object sender, EventArgs e)
    {
      UncheckAllOptions();
      SmallToolStripMenuItem.Checked = true;
      AdjustAllControls();
    }

    private void MediumToolStripMenuItemClick(object sender, EventArgs e)
    {
      UncheckAllOptions();
      MediumToolStripMenuItem.Checked = true;
      AdjustAllControls();
    }

    private void LargeToolStripMenuItemClick(object sender, EventArgs e)
    {
      UncheckAllOptions();
      LargeToolStripMenuItem.Checked = true;
      AdjustAllControls();
    }

    private static void AdjustControls(params Control[] listOfControls)
    {
      if (listOfControls.Length == 0)
      {
        return;
      }

      int position = listOfControls[0].Width + 33; // 33 is the initial padding
      bool isFirstControl = true;
      foreach (Control control in listOfControls)
      {
        if (isFirstControl)
        {
          isFirstControl = false;
        }
        else
        {
          control.Left = position + 10;
          position += control.Width;
        }
      }
    }

    private void AdjustAllControls()
    {
      AdjustControls(); // insert here all labels, textboxes and buttons, one method per line of controls
    }

    private void OptionsToolStripMenuItemClick(object sender, EventArgs e)
    {
      FormOptions frmOptions = new FormOptions(_configurationOptions);

      if (frmOptions.ShowDialog() == DialogResult.OK)
      {
        _configurationOptions = frmOptions.ConfigurationOptions2;
      }
    }

    private static void SetButtonEnabled(Button button, params Control[] controls)
    {
      bool result = true;
      foreach (Control ctrl in controls)
      {
        if (ctrl.GetType() == typeof(TextBox))
        {
          if (((TextBox)ctrl).Text == string.Empty)
          {
            result = false;
            break;
          }
        }

        if (ctrl.GetType() == typeof(ListView))
        {
          if (((ListView)ctrl).Items.Count == 0)
          {
            result = false;
            break;
          }
        }

        if (ctrl.GetType() == typeof(ComboBox))
        {
          if (((ComboBox)ctrl).SelectedIndex == -1)
          {
            result = false;
            break;
          }
        }
      }

      button.Enabled = result;
    }

    private void TextBoxNameKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        // do something
      }
    }

    private void ButtonStartNewGame_Click(object sender, EventArgs e)
    {
      EmptyAllbuttonText();
      InitializationOfTheBoard();

      // generate a new number
      int randomNumber = GenerateNewRandomNumber();
      int line = int.Parse(GenerateNewPosition().Split(':')[0]);
      int column = int.Parse(GenerateNewPosition().Split(':')[1]);
      board[line, column] = randomNumber;
      Displayboard(line, column);
      EnableSwipeMovement();
      DropTiles();
    }

    private void DropTiles()
    {
      //MoveTilesDown();
      //for (int i = 1; i < 9; i++)
      //{
      //  for (int j = 1; j < 9; j++)
      //  {
      //    if (board[i, j] != 0 && i != 8 && board[i + 1, j] == 0)
      //    {

      //    }

      //    if (i != 8 && board[i, j] == board[i + 1, j])
      //    {

      //    }
      //  }
    }

    /// <summary>
    /// Initialization of the board, all tiles will be set to zero.
    /// </summary>
    private void InitializationOfTheBoard()
    {
      for (int i = 1; i < 9; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          board[i, j] = 0;
        }
      }
    }

    private void EnableSwipeMovement()
    {
      buttonUp.Enabled = false;
      buttonDown.Enabled = false;
      buttonRight.Enabled = false;
      buttonLeft.Enabled = false;
      labelRight.Text = NumberOfTileMovableToTheRight().ToString();
      buttonRight.Enabled = NumberOfTileMovableToTheRight() > 0;

      labelLeft.Text = NumberOfTileMovableToTheLeft().ToString();
      buttonLeft.Enabled = NumberOfTileMovableToTheLeft() > 0;

      labelUp.Text = NumberOfTileMovableUp().ToString();
      if (NumberOfTileMovableUp() > 0)
      {
        buttonUp.Enabled = true;
      }
      else
      {
        buttonUp.Enabled = false;
      }

      //buttonUp.Enabled = NumberOfTileMovableUp() > 0;

      labelDown.Text = NumberOfTileMovableDown().ToString();
      buttonDown.Enabled = NumberOfTileMovableDown() > 0;
    }

    private void EmptyAllbuttonText()
    {
      button1.Text = string.Empty;
      button2.Text = string.Empty;
      button3.Text = string.Empty;
      button4.Text = string.Empty;
      button5.Text = string.Empty;
      button6.Text = string.Empty;
      button7.Text = string.Empty;
      button8.Text = string.Empty;
      button9.Text = string.Empty;
      button10.Text = string.Empty;
      button11.Text = string.Empty;
      button12.Text = string.Empty;
      button13.Text = string.Empty;
      button14.Text = string.Empty;
      button15.Text = string.Empty;
      button16.Text = string.Empty;
      button17.Text = string.Empty;
      button18.Text = string.Empty;
      button19.Text = string.Empty;
      button20.Text = string.Empty;
      button21.Text = string.Empty;
      button22.Text = string.Empty;
      button23.Text = string.Empty;
      button24.Text = string.Empty;
      button25.Text = string.Empty;
      button26.Text = string.Empty;
      button27.Text = string.Empty;
      button28.Text = string.Empty;
      button29.Text = string.Empty;
      button30.Text = string.Empty;
      button31.Text = string.Empty;
      button32.Text = string.Empty;
      button33.Text = string.Empty;
      button34.Text = string.Empty;
      button35.Text = string.Empty;
      button36.Text = string.Empty;
      button37.Text = string.Empty;
      button38.Text = string.Empty;
      button39.Text = string.Empty;
      button40.Text = string.Empty;
      button41.Text = string.Empty;
      button42.Text = string.Empty;
      button43.Text = string.Empty;
      button44.Text = string.Empty;
      button45.Text = string.Empty;
      button46.Text = string.Empty;
      button47.Text = string.Empty;
      button48.Text = string.Empty;
      button49.Text = string.Empty;
      button50.Text = string.Empty;
      button51.Text = string.Empty;
      button52.Text = string.Empty;
      button53.Text = string.Empty;
      button54.Text = string.Empty;
      button55.Text = string.Empty;
      button56.Text = string.Empty;
      button57.Text = string.Empty;
      button58.Text = string.Empty;
      button59.Text = string.Empty;
      button60.Text = string.Empty;
      button61.Text = string.Empty;
      button62.Text = string.Empty;
      button63.Text = string.Empty;
      button64.Text = string.Empty;
    }

    private int NumberOfTileMovableDown()
    {
      int result = 0;
      if (NumberOfTilesinColumn() == 0)
      {
        return result;
      }


      for (int i = 1; i < 9; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          if (board[i, j] != 0 && i != 8 && board[i + 1, j] == 0)
          {
            result++;
          }

          if (i != 8 && board[i, j] == board[i + 1, j])
          {
            result++;
          }
        }
      }

      return result;
    }

    private int NumberOfTilesinColumn(int columnNumber)
    {
      int result = 0;
      for (int i = 1; i <= 8; i++)
      {
        if (board[columnNumber, i] != 0)
        {
          result++;
        }
      }

      return result;
    }

    private int NumberOfTilesinLine(int lineNumber)
    {
      int result = 0;
      for (int i = 1; i <= 8; i++)
      {
        if (board[i, lineNumber] != 0)
        {
          result++;
        }
      }

      return result;
    }

    private int NumberOfTileMovableUp()
    {
      int result = 0;
      if (NumberOfTileInTopRow() == 0) // false to be fixed
      {
        return result;
      }

      for (int i = 1; i < 9; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          if (board[i, j] != 0 && i != 1 && board[i - 1, j] == 0)
          {
            result++;
          }

          if (i != 1 && board[i, j] == board[i - 1, j])
          {
            result++;
          }
        }
      }

      return result;
    }

    private int NumberOfTileInTopRow()
    {
      int result = 0;
      for (int i = 1; i <= 8; i++)
      {
        if (board[i, 1] == 0 || board[i, 1] == board[i, 2])
        {
          result++;
        }

      }

      return result;
    }

    private int NumberOfTileMovableToTheLeft()
    {
      int result = 0;
      for (int i = 1; i < 9; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          if (board[i, j] != 0 && j != 1 && board[i, j - 1] == 0)
          {
            result++;
          }

          if (j != 1 && board[i, j] == board[i, j - 1])
          {
            result++;
          }
        }
      }

      return result;
    }

    private int NumberOfTileMovableToTheRight()
    {
      int result = 0;
      for (int i = 1; i < 9; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          if (board[i, j] != 0 && j != 8 && board[i, j + 1] == 0)
          {
            result++;
          }

          if (j != 8 && board[i, j] == board[i, j + 1])
          {
            result++;
          }
        }
      }

      return result;
    }

    private void Displayboard(int line, int column)
    {
      switch ($"{line}:{column}")
      {
        case "1:1":
          button1.Text = board[line, column].ToString();
          break;
        case "1:2":
          button2.Text = board[line, column].ToString();
          break;
        case "1:3":
          button3.Text = board[line, column].ToString();
          break;
        case "1:4":
          button4.Text = board[line, column].ToString();
          break;
        case "1:5":
          button5.Text = board[line, column].ToString();
          break;
        case "1:6":
          button6.Text = board[line, column].ToString();
          break;
        case "1:7":
          button7.Text = board[line, column].ToString();
          break;
        case "1:8":
          button8.Text = board[line, column].ToString();
          break;
        case "2:1":
          button9.Text = board[line, column].ToString();
          break;
        case "2:2":
          button10.Text = board[line, column].ToString();
          break;
        case "2:3":
          button11.Text = board[line, column].ToString();
          break;
        case "2:4":
          button12.Text = board[line, column].ToString();
          break;
        case "2:5":
          button13.Text = board[line, column].ToString();
          break;
        case "2:6":
          button14.Text = board[line, column].ToString();
          break;
        case "2:7":
          button15.Text = board[line, column].ToString();
          break;
        case "2:8":
          button16.Text = board[line, column].ToString();
          break;
        case "3:1":
          button17.Text = board[line, column].ToString();
          break;
        case "3:2":
          button18.Text = board[line, column].ToString();
          break;
        case "3:3":
          button19.Text = board[line, column].ToString();
          break;
        case "3:4":
          button20.Text = board[line, column].ToString();
          break;
        case "3:5":
          button21.Text = board[line, column].ToString();
          break;
        case "3:6":
          button22.Text = board[line, column].ToString();
          break;
        case "3:7":
          button23.Text = board[line, column].ToString();
          break;
        case "3:8":
          button24.Text = board[line, column].ToString();
          break;
        case "4:1":
          button25.Text = board[line, column].ToString();
          break;
        case "4:2":
          button26.Text = board[line, column].ToString();
          break;
        case "4:3":
          button27.Text = board[line, column].ToString();
          break;
        case "4:4":
          button28.Text = board[line, column].ToString();
          break;
        case "4:5":
          button29.Text = board[line, column].ToString();
          break;
        case "4:6":
          button30.Text = board[line, column].ToString();
          break;
        case "4:7":
          button31.Text = board[line, column].ToString();
          break;
        case "4:8":
          button32.Text = board[line, column].ToString();
          break;
        case "5:1":
          button33.Text = board[line, column].ToString();
          break;
        case "5:2":
          button34.Text = board[line, column].ToString();
          break;
        case "5:3":
          button35.Text = board[line, column].ToString();
          break;
        case "5:4":
          button36.Text = board[line, column].ToString();
          break;
        case "5:5":
          button37.Text = board[line, column].ToString();
          break;
        case "5:6":
          button38.Text = board[line, column].ToString();
          break;
        case "5:7":
          button39.Text = board[line, column].ToString();
          break;
        case "5:8":
          button40.Text = board[line, column].ToString();
          break;
        case "6:1":
          button41.Text = board[line, column].ToString();
          break;
        case "6:2":
          button42.Text = board[line, column].ToString();
          break;
        case "6:3":
          button43.Text = board[line, column].ToString();
          break;
        case "6:4":
          button44.Text = board[line, column].ToString();
          break;
        case "6:5":
          button45.Text = board[line, column].ToString();
          break;
        case "6:6":
          button46.Text = board[line, column].ToString();
          break;
        case "6:7":
          button47.Text = board[line, column].ToString();
          break;
        case "6:8":
          button48.Text = board[line, column].ToString();
          break;
        case "7:1":
          button49.Text = board[line, column].ToString();
          break;
        case "7:2":
          button50.Text = board[line, column].ToString();
          break;
        case "7:3":
          button51.Text = board[line, column].ToString();
          break;
        case "7:4":
          button52.Text = board[line, column].ToString();
          break;
        case "7:5":
          button53.Text = board[line, column].ToString();
          break;
        case "7:6":
          button54.Text = board[line, column].ToString();
          break;
        case "7:7":
          button55.Text = board[line, column].ToString();
          break;
        case "7:8":
          button56.Text = board[line, column].ToString();
          break;
        case "8:1":
          button57.Text = board[line, column].ToString();
          break;
        case "8:2":
          button58.Text = board[line, column].ToString();
          break;
        case "8:3":
          button59.Text = board[line, column].ToString();
          break;
        case "8:4":
          button60.Text = board[line, column].ToString();
          break;
        case "8:5":
          button61.Text = board[line, column].ToString();
          break;
        case "8:6":
          button62.Text = board[line, column].ToString();
          break;
        case "8:7":
          button63.Text = board[line, column].ToString();
          break;
        case "8:8":
          button64.Text = board[line, column].ToString();
          break;
        default:
          // don't do anything because outside the board
          break;
      }
    }

    public static int GenerateNewRandomNumber()
    {
      int result = GenerateRndNumberUsingCrypto(1, 254);
      // more twos than fours
      if (result < 245)
      {
        result = 2;
      }
      else
      {
        result = 4;
      }

      return result;
    }

    private string GenerateNewPosition()
    {
      string result = "0:0";
      int column = GenerateRndNumberUsingCrypto(1, 8);
      int line = GenerateRndNumberUsingCrypto(1, 8);
      do
      {
        column = GenerateRndNumberUsingCrypto(1, 8);
        line = GenerateRndNumberUsingCrypto(1, 8);
      } while (board[line, column] != 0);

      result = $"{line}:{column}";
      return result;
    }

    public static int GenerateRndNumberUsingCrypto(int min, int max)
    {
      if (max > 255 || min < 0)
      {
        return 0;
      }

      if (max == min)
      {
        return min;
      }

      int result;
      var crypto = new RNGCryptoServiceProvider();
      byte[] randomNumber = new byte[1];
      do
      {
        crypto.GetBytes(randomNumber);
        result = randomNumber[0];
      } while (result < min || result > max);

      return result;
    }

    private void ButtonUp_Click(object sender, EventArgs e)
    {

    }

    private void ButtonDown_Click(object sender, EventArgs e)
    {
      // move all tiles down when possible
      for (int column = 1; column < 9; column++)
      {
        for (int line = 8; line > 1; line--)
        {
          if (board[line, column] == board[line - 1, column])
          {
            MoveTilesDown(line, column);

          }
        }
      }
    }

    private void MoveTilesDown(int line, int column)
    {
      board[line, column] *= 2;
      // move other tiles down
      // TODO
      for (int i = line - 1; i > 1; i--)
      {
        if (board[i, column] == 0)
        {
          // TODO
        }
      }
    }

    private void ButtonRight_Click(object sender, EventArgs e)
    {

    }

    private void ButtonLeft_Click(object sender, EventArgs e)
    {

    }

    private void ColorizeBackgroundTile(int line, int column)
    {
      Color backgroundColor = ColorizeTile(board[line, column]);
      switch ($"{line}:{column}")
      {
        case "1:1":
          button1.BackColor = backgroundColor;
          break;
        case "1:2":
          button2.BackColor = backgroundColor;
          break;

        default:
          break;
      }
    }

    public static Color ColorizeTile(int number)
    {
      switch (number)
      {
        case 2:
          return Color.White;
        case 4:
          return Color.FromArgb(50, 50, 50);
        case 8:
          return Color.Orange;
        case 16:
          return Color.OrangeRed;
        case 32:
          return Color.DarkOrange;
        case 64:
          return Color.IndianRed;
        case 128:
          return Color.Yellow;
        case 256:
          return Color.YellowGreen;
        case 512:
          return Color.LightYellow;
        case 1024:
          return Color.LightGoldenrodYellow;
        case 2048:
          return Color.GreenYellow;
        case 4096:
          return Color.Green;
        case 8192:
          return Color.LightGreen;
        case 16384:
          return Color.ForestGreen;
        case 32768:
          return Color.DarkOliveGreen;
        case 65536:
          return Color.DarkSeaGreen;
        default:
          return Color.White;
      }
    }
  }
}