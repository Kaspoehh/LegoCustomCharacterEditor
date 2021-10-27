using System.Collections;
using System.IO;
using System.Runtime;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class LEGOAddCharacterEditor : EditorWindow
{
    private string _characterName;
    
    private Texture2D _faceFront;
    private Texture2D _torsoFront;
    private Texture2D _torsoBack;
    private Texture2D _hip_Front;
    private Texture2D _hip_Crotch;
    private Texture2D _leg_L_Foot;
    private Texture2D _leg_L_Front;
    private Texture2D _leg_R_Foot;
    private Texture2D _leg_R_Front;

    private Color _torsoBase;
    private Color _hip_main;
    private Color _leg_main;
    private Color _armBase;
    private Color _handBase;

    private GameObject _prefab;

    private Texture2D _bodyMecabricksTexture;
    private Texture2D _faceMecabricksTexture;

    [MenuItem("LEGO Tools/TestWindowCharacter")]
    public static void ShowWindow()
    {
        GetWindow<LEGOAddCharacterEditor>("TestWindowCharacter");
    }

    private void OnGUI()
    {
        _characterName = EditorGUILayout.TextField("Character Name: ", _characterName);

        CreateTextureFields();
        CreateColorFields();

        _prefab = PrefabField("Default Prefab", _prefab);

        if (GUILayout.Button("Make Custom LEGO Character"))
        {
            if (_characterName != "")
            {
                Debug.Log(Application.dataPath);
                string parentFolderName = (Application.dataPath + "/LEGO Custom Editors/CustomCharacters");
                if (!Directory.Exists(parentFolderName + "/" + _characterName + "/materials"))
                {
                    string characterFolder = parentFolderName + "/" + _characterName;
                    
                    if(!Directory.Exists(parentFolderName + "/" + _characterName))
                    {
                        Directory.CreateDirectory(parentFolderName + "/" + _characterName);
                    }
                    
                    string charFol = ("Assets/LEGO Custom Editors/CustomCharacters/" + _characterName);

                    Directory.CreateDirectory(characterFolder + "/materials");
                    AssetDatabase.Refresh();
                    CreateMaterial(charFol + "/materials", 0.1F);
                }
                else
                {
                    Debug.LogError("Already an character with the same name");
                }

            }
            else
            {
                Debug.LogWarning("Custom Character Name Set");
            }

        }

        GUILayout.BeginHorizontal();
        _bodyMecabricksTexture = TextureField("Mecabricks body", _bodyMecabricksTexture);
        _faceMecabricksTexture = TextureField("Mecabricks face", _faceMecabricksTexture);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Cut out Mecabricks Textures"))
        {
            string parentFolderName = (Application.dataPath + "/LEGO Custom Editors/CustomCharacters");
            if (!Directory.Exists(parentFolderName + "/" + _characterName + "/textures"))
            {
                Directory.CreateDirectory(parentFolderName + "/" + _characterName + "/textures");

                AssetDatabase.Refresh();
                CreateTexFromMecaBrick(_bodyMecabricksTexture, "frontBody" + _characterName, 284, 559);
                CreateTexFromMecaBrick(_bodyMecabricksTexture, "backBody" + _characterName, 284, 84);
            }
            else
            {
                Debug.LogError("Already an character with the same name, create an new name or delete the old character folder");
            }
            // CreateTexFromMecaBrick(_faceMecabricksTexture, "face" + _characterName);
        }
    }

    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        // style.alignment = TextAnchor.UpperCenter;
        // style.fixedWidth = 70;
        GUILayout.Label(name);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }

    private static GameObject PrefabField(string name, GameObject prefab)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (GameObject)EditorGUILayout.ObjectField(prefab, typeof(GameObject), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }

    private void CreateTextureFields()
    {
        EditorGUILayout.BeginHorizontal();
        _torsoFront = TextureField("Front", _torsoFront);
        _torsoBack = TextureField("Back", _torsoBack);
        _faceFront = TextureField("Face Front   ", _faceFront);
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.BeginHorizontal();
        _hip_Front = TextureField("Hip", _hip_Front);
        _hip_Crotch = TextureField("Crotch", _hip_Crotch);
        _leg_L_Foot = TextureField("L. Foot", _leg_L_Foot);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        _leg_L_Front = TextureField("L. Leg Front Top", _leg_L_Front);
        _leg_R_Foot = TextureField("R. Foot", _leg_R_Foot);
        _leg_R_Front = TextureField("R. Leg Front Top", _leg_R_Front);
        EditorGUILayout.EndHorizontal();

    }

    private void CreateColorFields()
    {
        _torsoBase = EditorGUILayout.ColorField("Torso Base", _torsoBase);
        _hip_main = EditorGUILayout.ColorField("Hip Base", _hip_main);
        _leg_main = EditorGUILayout.ColorField("Leg Base", _leg_main);
        _armBase = EditorGUILayout.ColorField("Arm Base", _armBase);
        _handBase = EditorGUILayout.ColorField("Hand Base", _handBase);
    }

    private async void CreateMaterial(string savePath, float waitTime)
    {
        GameObject prefabInst = Instantiate(_prefab);

        GameObject faceGO = prefabInst.transform.GetChild(0).GetChild(2).gameObject;
        
        GameObject torsoBaseGO = prefabInst.transform.GetChild(0).GetChild(9).GetChild(2).gameObject;
        GameObject torsoGO = prefabInst.transform.GetChild(0).GetChild(9).GetChild(1).gameObject;
        GameObject backGO = prefabInst.transform.GetChild(0).GetChild(9).GetChild(0).gameObject;
    
        GameObject hipBaseGO = prefabInst.transform.GetChild(0).GetChild(6).GetChild(2).gameObject;
        GameObject hipFrontGO = prefabInst.transform.GetChild(0).GetChild(6).GetChild(1).gameObject;
        GameObject crotchGO = prefabInst.transform.GetChild(0).GetChild(6).GetChild(0).gameObject;
        
        GameObject legLMainGO = prefabInst.transform.GetChild(0).GetChild(7).GetChild(2).gameObject;
        GameObject legLSideGO = prefabInst.transform.GetChild(0).GetChild(7).GetChild(3).gameObject;
        GameObject legLFootGO = prefabInst.transform.GetChild(0).GetChild(7).GetChild(0).gameObject;
        GameObject legLFrontGO = prefabInst.transform.GetChild(0).GetChild(7).GetChild(1).gameObject;
       
        GameObject legRMainGO = prefabInst.transform.GetChild(0).GetChild(8).GetChild(2).gameObject;
        GameObject legRSideGO = prefabInst.transform.GetChild(0).GetChild(8).GetChild(3).gameObject;
        GameObject legRFootGO = prefabInst.transform.GetChild(0).GetChild(8).GetChild(0).gameObject;
        GameObject legRFrontGO = prefabInst.transform.GetChild(0).GetChild(8).GetChild(1).gameObject;
        
        GameObject armLeftMainGO = prefabInst.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        GameObject armLeftFrontGO = prefabInst.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        
        GameObject armRightMainGO = prefabInst.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        GameObject armRightFrontGO = prefabInst.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;

        CreateMaterials(_faceFront, savePath, "faceMat", faceGO, true);
        
        CreateMaterials(_torsoBase, savePath, "torsoBaseMat", torsoBaseGO);
        
        CreateMaterials(_torsoFront, savePath, "torsoFrontMat", torsoGO,false);
        
        CreateMaterials(_torsoBack, savePath, "torsoBackMat", backGO,false);
        
        CreateMaterials(_hip_main, savePath, "hipMat", hipBaseGO);
        
        CreateMaterials(_hip_Front, savePath, "hipMat", hipFrontGO,false);
        
        CreateMaterials(_hip_Crotch, savePath, "crotchMat", crotchGO,false);
        
        
        CreateMaterials(_leg_main, savePath, "legLMainMat", legLMainGO);
        
        CreateMaterials(_leg_main, savePath, "legLSideMat", legLSideGO);
        
        CreateMaterials(_leg_L_Foot, savePath, "footLMat", legLFootGO,false);
        
        CreateMaterials(_leg_L_Front, savePath, "legLMat", legLFrontGO,false);
        
        
        CreateMaterials(_leg_main, savePath, "legRMainMat", legRMainGO);
        
        CreateMaterials(_leg_main, savePath, "legRSideMat", legRSideGO);
        CreateMaterials(_leg_R_Foot, savePath, "footRMat", legRFootGO,false);
        CreateMaterials(_leg_R_Front, savePath, "legRMat", legRFrontGO,false);
        
        CreateMaterials(_armBase, savePath, "armLMainMat", armLeftMainGO);
        CreateMaterials(_armBase, savePath, "armLFrontMat", armLeftFrontGO);
            CreateMaterials(_armBase, savePath, "armRMainMat", armRightMainGO);
            CreateMaterials(_armBase, savePath, "armRFrontMat", armRightFrontGO);

        AssetDatabase.SaveAssets();
    }

    private async void CreateMaterials(Texture2D textureInput,string savePath, string saveName, GameObject goToApplyTexture, bool renderTransparent)
    { 
        Material mat = new Material(Shader.Find("Standard"));

        mat.mainTexture = textureInput;

        if (renderTransparent)
        {     
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }     
        
        AssetDatabase.Refresh();
        AssetDatabase.CreateAsset(mat, savePath +"/"+ saveName +".mat");
        
        AssetDatabase.Refresh();
        SetSkinnedMat(goToApplyTexture, 0, mat);  


    }
    
    private void CreateMaterials(Color colorInput,string savePath, string saveName, GameObject goToApplyTexture)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = (colorInput);
        AssetDatabase.CreateAsset(mat, savePath +"/"+ saveName +".mat");
        SetSkinnedMat(goToApplyTexture, 0, mat);  
    }
    
    void SetSkinnedMat(GameObject obj, int Mat_Nr, Material Mat)
    {
        SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
        renderer.sharedMaterial = Mat;
    }

    private void CreateTexFromMecaBrick(Texture2D textureInput, string saveName, int textStartX, int texStartY)
    {
        Texture2D tex = null;
        
        
        tex = new Texture2D(453, 381,TextureFormat.ARGB32, false);
        for (int y = 0; y < 381; y++)
        {
                for (int x = 0; x < 453; x++)
                {
                    //backbody
                    //284
                    //84
                    Color pixelColor = textureInput.GetPixel(x + textStartX,y + texStartY);
                    tex.SetPixel(x, y, pixelColor);
                }
        }
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        
        File.WriteAllBytes(
        "Assets/LEGO Custom Editors/CustomCharacters/" + 
        _characterName + "/textures/"+
        saveName + ".png", bytes);        
        AssetDatabase.Refresh();
    
        //TODO: SAVE TEXTURE AND PARSE THAT PNG BACK TO AN TEX AND APPLY THAT
    }

}

