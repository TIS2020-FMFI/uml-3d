using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using OALProgramControl;
using Assets.Scripts.Animation;
using System.IO;
using System.Text;

public class MenuManager : Singleton<MenuManager>
{

    FileLoader fileLoader;

    //UI Panels
    //------------------------- my Code
    [SerializeField]
    private GameObject panelSavedCode;
    [SerializeField]
    public TMP_InputField savedScript;
    [SerializeField]
    public TMP_InputField methodCode;
    [SerializeField]
    private TMP_Text methodCaption;
    [SerializeField]
    private GameObject panelBody;
    [SerializeField]
    private GameObject debugWindow;
    [SerializeField]
    private TMP_InputField debugField;
    [SerializeField]
    private TMP_Text debugName;

    private bool isAnimating = true;
    private string methodBodyName;
    private string methodBodyClass;
    //------------------------- my Code
    [SerializeField]
    private GameObject introScreen;
    [SerializeField]
    private GameObject animationScreen;
    [SerializeField]
    private GameObject mainScreen;
    [SerializeField]
    private Button saveBtn;
    [SerializeField]
    private TMP_Dropdown animationsDropdown;
    [SerializeField]
    private TMP_InputField scriptCode;
    [SerializeField]
    private GameObject PanelColors;
    [SerializeField]
    private GameObject PanelInteractiveIntro;
    [SerializeField]
    private GameObject PanelInteractive;
    [SerializeField]
    private GameObject PanelMethod;
    public bool isCreating=false;
    [SerializeField]
    private List<GameObject> methodButtons;
    [SerializeField]
    private TMP_Text ClassNameTxt;
    [SerializeField]
    private GameObject InteractiveText;
    [SerializeField]
    private GameObject PanelInteractiveShow;
    [SerializeField]
    private TMP_Text classFromTxt;
    [SerializeField]
    private TMP_Text classToTxt;
    [SerializeField]
    private TMP_Text methodFromTxt;
    [SerializeField]
    private TMP_Text methodToTxt;
    [SerializeField]
    private GameObject PanelInteractiveCompleted;
    [SerializeField]
    private Slider speedSlider;
    [SerializeField]
    private TMP_Text speedLabel;
    private string interactiveSource = "source";
    private string sourceClassName = "";
    [SerializeField]
    public GameObject panelError;
    [SerializeField]
    public GameObject panelAnimationPlay;
    [SerializeField]
    public GameObject panelStepMode;
    [SerializeField]
    public GameObject panelPlayMode;
    struct InteractiveData
    {
        public string fromClass;
        public string fromMethod;
        public string relationshipName;
        public string toClass;
        public string toMethod;
    }
    InteractiveData interactiveData = new InteractiveData();
    private void Awake()
    {
        fileLoader = GameObject.Find("FileManager").GetComponent<FileLoader>();
    }
    private void Start()
    {
        UpdateSpeed();
    }
    //Update the list of created animations
    public void UpdateAnimations()
    {
        List<string> options = new List<string>();
        foreach (Anim anim in AnimationData.Instance.getAnimList())
        {
            options.Add(anim.AnimationName);
        }
        animationsDropdown.ClearOptions();
        animationsDropdown.AddOptions(options);
    }
    public void SetSelectedAnimation(string name)
    {
        animationsDropdown.value= animationsDropdown.options.FindIndex(option => option.text == name);
    }
    public void StartAnimate()
    {
        InteractiveText.GetComponent<DotsAnimation>().currentText= "Select source class\n for call function\ndirectly in diagram\n.";
        scriptCode.text = "";
        methodCode.text = "";
        OALScriptBuilder.GetInstance().Clear();
        InteractiveData interactiveData = new InteractiveData();
        isCreating = true;
        introScreen.SetActive(false);
        PanelInteractiveIntro.SetActive(true);
        PanelMethod.SetActive(false);
        PanelInteractive.SetActive(true);
        PanelInteractiveCompleted.SetActive(false);
        SwitchToAnimate();
        mainScreen.SetActive(false);
    }

    public void ResetData()
    {
        Animation.Instance.UnhighlightAll();
        interactiveData.fromClass = null;
        interactiveData.fromMethod = null;
        interactiveData.relationshipName = null;
        interactiveData.toClass = null;
        interactiveData.toMethod = null;
    }

    public void SwitchToAnimate()
    {
        animationScreen.SetActive(true);
        panelBody.SetActive(false);
        isAnimating = true;
        ResetData();
    }

    public void SwitchToMethod()
    {
        panelBody.SetActive(true);
        animationScreen.SetActive(false);
        isAnimating = false;
        if (methodBodyName != null)
        {
            methodCode.text = readMethodCode();
        }
        ResetData();
    }

    public string readMethodCode()
    {
        try
        {
            return File.ReadAllText(@"Methods/" + methodBodyName + ".oal");
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
            return "";
        }
    }

    public void EndAnimate()
    {
        isCreating = false;
        PanelInteractiveIntro.SetActive(false);
        PanelInteractive.SetActive(false);
        animationScreen.SetActive(false);
        panelBody.SetActive(false);
        saveBtn.interactable = false;
        mainScreen.SetActive(true);
        introScreen.SetActive(true);
        PanelInteractiveCompleted.SetActive(false);
        PanelInteractiveShow.SetActive(false);
    }
    public void SelectClass(String name)
    {
        foreach(GameObject button in methodButtons)
        {
            button.SetActive(false);
        } 
        Class selectedClass = ClassDiagram.Instance.FindClassByName(name);
        PanelInteractiveIntro.SetActive(false);
        ClassNameTxt.text = name;
        PanelMethod.SetActive(true);
        int i = 0;
        if (selectedClass.Methods != null)
        {
            foreach (Method m in selectedClass.Methods)
            {
                if (isAnimating == false)
                {
                    methodBodyClass = name;
                    if (i < methodButtons.Count)
                    {
                        methodButtons[i].SetActive(true);
                        methodButtons[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                    }
                    if (interactiveData.fromClass != null)
                    {
                        Animation.Instance.HighlightClass(interactiveData.fromClass, false);
                    }
                    if (interactiveData.fromMethod != null)
                    {
                        Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, false);
                    }
                    interactiveData.fromMethod = null;
                    interactiveData.fromClass = name;
                    Animation.Instance.HighlightClass(interactiveData.fromClass, true);
                    i++;
                } 
                else
                {   
                    if (interactiveData.fromMethod == null)
                    {
                        if (i < methodButtons.Count)
                        {
                            methodButtons[i].SetActive(true);
                            methodButtons[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                        }
                        if (interactiveData.fromClass != null)
                        {
                            Animation.Instance.HighlightClass(interactiveData.fromClass, false);
                        }
                        interactiveData.fromClass = name;
                        Animation.Instance.HighlightClass(interactiveData.fromClass, true);
                        i++;
                    }
                    else
                    {
                        if (i < methodButtons.Count && ClassDiagram.Instance.FindEdge(interactiveData.fromClass, name) != null)
                        {
                            methodButtons[i].SetActive(true);
                            methodButtons[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                        }
                        if (interactiveData.toClass != null)
                        {
                            Animation.Instance.HighlightClass(interactiveData.toClass, false);
                        }
                        interactiveData.toClass = name;
                        Animation.Instance.HighlightClass(interactiveData.toClass, true);
                        i++;
                    }
                }
            }
        }
        UpdateInteractiveShow();
        PanelInteractiveIntro.SetActive(false);
        PanelMethod.SetActive(true);
    }
    public void SelectMethod(int buttonID)
    {
        if (isAnimating == false)
        {
            if (interactiveData.fromMethod == null)
            {
                string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
                methodBodyName = methodBodyClass + '_' + methodName;
                string[] tempText = methodBodyName.Split('_');
                methodCaption.text = tempText[0]+'\n'+tempText[1];
                InteractiveText.GetComponent<DotsAnimation>().currentText = "Select target class\nfor call function\ndirectly in diagram\n.";
                interactiveData.fromMethod = methodName;
                Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, true);
                methodCode.text = readMethodCode();
                UpdateInteractiveShow();
            }
        } 
        else
        {
            if (interactiveData.fromMethod == null)
            {
                string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
                InteractiveText.GetComponent<DotsAnimation>().currentText = "Select target class\nfor call function\ndirectly in diagram\n.";
                interactiveData.fromMethod = methodName;
                Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, true);
                UpdateInteractiveShow();
            }
            else
            {
                string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
                InteractiveText.GetComponent<DotsAnimation>().currentText = "Select source class\nfor call function\ndirectly in diagram\n.";
                interactiveData.toMethod = methodName;
                UpdateInteractiveShow();
                Animation.Instance.HighlightClass(interactiveData.fromClass, false);
                Animation.Instance.HighlightClass(interactiveData.toClass, false);
                Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, false);
                WriteCode();
            }
        }

        
        PanelInteractiveIntro.SetActive(true);
        PanelMethod.SetActive(false);
    }
    private void WriteCode()
    {
        if (!scriptCode.text.EndsWith("\n") && scriptCode.text.Length > 1)
            scriptCode.text += "\n";
            scriptCode.text += OALScriptBuilder.GetInstance().AddCall(
                interactiveData.fromClass, interactiveData.fromMethod,
                OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(interactiveData.fromClass, interactiveData.toClass).RelationshipName, interactiveData.toClass,
                interactiveData.toMethod
            );

        interactiveData = new InteractiveData();
    }

    //Save animation to file and memory
    public void SaveAnimation()
    {
        scriptCode.GetComponent<CodeHighlighter>().RemoveColors();
        Anim newAnim = new Anim("", scriptCode.text);
        fileLoader.SaveAnimation(newAnim);
        Animation.Instance.UnhighlightAll();
        EndAnimate();
    }

    public void SaveMethod()
    {
        methodCode.GetComponent<CodeHighlighter>().RemoveColors();
        MethodBody newMethod = new MethodBody("", methodCode.text);
        
        Animation.Instance.UnhighlightAll();
        string[] lines = methodCode.text.Split('\n');
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"Methods/" + methodBodyName + ".oal"))
        {
            foreach (string line in lines)
            {
                file.WriteLine(line);
            }
        }
        EndAnimate();
    }
    public void SelectAnimation()
    {
         String name= animationsDropdown.options[animationsDropdown.value].text;
        foreach(Anim anim in AnimationData.Instance.getAnimList())
        {
            if (name.Equals(anim.AnimationName))
                AnimationData.Instance.selectedAnim = anim;
        }
    }

    public void OpenAnimation()
    {
        if (AnimationData.Instance.getAnimList().Count > 0)
        {
            SelectAnimation();
            StartAnimate();
            scriptCode.text = AnimationData.Instance.selectedAnim.Code;
            AnimationData.Instance.RemoveAnim(AnimationData.Instance.selectedAnim);
            UpdateAnimations();
        }
    }
    public void ActivatePanelColors(bool show)
    {
        PanelColors.SetActive(show);
    }

    public void UpdateInteractiveShow()
    {
        PanelInteractiveCompleted.SetActive(false);
        PanelInteractiveShow.SetActive(true);
        classFromTxt.text = "Class: ";
        methodFromTxt.text = "Method: ";
        classToTxt.text = "Class: ";
        if (interactiveData.fromClass != null)
        {
            classFromTxt.text = "Class: " + interactiveData.fromClass;
        }
        if (interactiveData.fromMethod != null)
        {
            methodFromTxt.text = "Method: " + interactiveData.fromMethod;
        }
        if (interactiveData.toClass != null)
        {
            classToTxt.text = "Class: " + interactiveData.toClass;
        }
        if (interactiveData.toMethod != null)
        {
            PanelInteractiveCompleted.SetActive(true);
        }
    }
    public void UpdateSpeed()
    {
        AnimationData.Instance.AnimSpeed = speedSlider.value;
        speedLabel.text = speedSlider.value.ToString()+"s";

    }
    public void PlayAnimation()
    {
        if (AnimationData.Instance.selectedAnim.AnimationName.Equals(""))
        {
            panelError.SetActive(true);
        }
        else
        {
            panelAnimationPlay.SetActive(true);
            mainScreen.SetActive(false);
            //My code------------
            panelSavedCode.SetActive(true);
            introScreen.SetActive(false);
            debugWindow.SetActive(true);
            debugField.text = "";
            savedScript.text = AnimationData.Instance.selectedAnim.Code;
            Animation.Instance.script = savedScript;
            string checkFile = "Client::startVisitorScenario()";
            if (AnimationData.Instance.selectedAnim.Code == checkFile)
            {
                AnimationScriptCreator.Instance.fileName = checkFile;
                AnimationScriptCreator.Instance.initConstructing();
                savedScript.text = AnimationScriptCreator.Instance.ScriptText;

                string code = savedScript.text;
                Anim loadedAnim = new Anim("Client::startVisitorScenario()", code);
                scriptCode.text = savedScript.text;
                AnimationData.Instance.selectedAnim = loadedAnim;
                
                Animation.Instance.script = savedScript;
            }
            //My code------------
            if (Animation.Instance.standardPlayMode)
            {
                panelStepMode.SetActive(false);
                panelPlayMode.SetActive(true);
            }
            else
            {
                panelPlayMode.SetActive(false);
                panelStepMode.SetActive(true);
            }
        }
    }
    public void ResetInteractiveSelection()
    {   if(interactiveData.fromClass!=null)
            Animation.Instance.HighlightClass(interactiveData.fromClass, false);
        if (interactiveData.toClass != null)
            Animation.Instance.HighlightClass(interactiveData.toClass, false);
        if (interactiveData.fromMethod != null)
            Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, false);
        InteractiveText.GetComponent<DotsAnimation>().currentText = "Select source class\nfor call function\ndirectly in diagram\n.";
        interactiveData = new InteractiveData();
        UpdateInteractiveShow();
        PanelInteractiveIntro.SetActive(true);
        PanelMethod.SetActive(false);
    }
    public void ChangeMode()
    {
        Animation.Instance.UnhighlightAll();
        Animation.Instance.isPaused = false;
        if (Animation.Instance.standardPlayMode)
        {
            Animation.Instance.standardPlayMode = false;
            panelPlayMode.SetActive(false);
            panelStepMode.SetActive(true);
        }
        else
        {
            Animation.Instance.standardPlayMode = true;
            panelStepMode.SetActive(false);
            panelPlayMode.SetActive(true);
        }
    }

    public void fillDebugWindow(String name)
    {

        if (name == "NULL")
        {
            debugName.text = "Debug Window";
            debugField.text = "";
            return;
        }
        string[] nameFormat = name.Split('_');
        string ClassName = nameFormat[0];
        Class selectedClass = ClassDiagram.Instance.FindClassByName(ClassName);
        string atrributes = "\nAributes:\n";
        if (selectedClass.Attributes != null)
        {
            foreach (Attribute a in selectedClass.Attributes)
            {
                atrributes += a.Name;
                atrributes += ": ";
                atrributes += a.Type;
                atrributes += '\n';
            }
        }
        debugName.text = "Class: " + nameFormat[0] + '\n' + "Method: " + nameFormat[1] + '\n' + atrributes;
        try
        {
            debugField.text = File.ReadAllText(@"Methods/" + name + ".oal");
        }
        catch (IOException e)
        {
            debugField.text = "";
        }
    }
}
