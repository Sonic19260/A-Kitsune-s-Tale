﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveMenuHandler : MonoBehaviour
{
    [SerializeField]
    private List<Button> saveButtons;
    [SerializeField]
    private TMP_InputField usernameField;
    [SerializeField]
    private GameObject buttonsParent;
    [SerializeField]
    private GameObject cancelButton;
    [SerializeField]
    private GameObject deleteButton;

    private enum SaveFileMode {
        NEW_GAME,
        CONTINUE,
        NULL
    }

    private SaveFileMode saveMode;

    private void Start()
    {
        saveMode = SaveFileMode.NULL;
        for (int i = 0; i < saveButtons.Count; i++) {
            string filePath = Application.persistentDataPath + "/save_data_" + (i + 1) + ".dat";
            FileStream file;

            if (File.Exists(filePath) == false) {
                saveButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "No Data";
            } else {
                file = File.OpenRead(filePath);

                BinaryFormatter bf = new BinaryFormatter();
                ArrayList data = (ArrayList)bf.Deserialize(file);
                file.Close();

                saveButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = (string)data[0];
            }
        }
    }

    public void ChangeSaveFileMode(string mode) {
        switch (mode) {
            case "New Game": {
                saveMode = SaveFileMode.NEW_GAME;
                break;
            }

            case "Continue": {
                saveMode = SaveFileMode.CONTINUE;
                break;
            }
            
            default: {
                saveMode = SaveFileMode.NULL;
                break;
            }
        }
    }

    public void PrepareBeginGame(int saveFile) {
        if (Utils.saveFile != 0) {
            string filePath = Application.persistentDataPath + "/save_data_" + Utils.saveFile + ".dat";
            FileStream file;

            if (File.Exists(filePath) == false) {
                saveButtons[Utils.saveFile - 1].GetComponentInChildren<TextMeshProUGUI>().text = "No Data";
            } else {
                file = File.OpenRead(filePath);

                BinaryFormatter bf = new BinaryFormatter();
                ArrayList data = (ArrayList)bf.Deserialize(file);
                file.Close();

                saveButtons[Utils.saveFile - 1].GetComponentInChildren<TextMeshProUGUI>().text = (string)data[0];
            }
        }

        switch(saveMode) {
            case SaveFileMode.NEW_GAME: {
                string filePath = Application.persistentDataPath + "/save_data_" + saveFile + ".dat";
                FileStream file;

                if (File.Exists(filePath) == true) {
                    return;
                }

                usernameField.transform.position = saveButtons[saveFile - 1].transform.position;
                saveButtons[saveFile - 1].GetComponentInChildren<TextMeshProUGUI>().text = "";
                usernameField.gameObject.SetActive(true);
                usernameField.ActivateInputField();
                AdjustButtonsParentPosition(saveFile);
                cancelButton.SetActive(true);
                buttonsParent.SetActive(true);

                Utils.saveFile = saveFile;

                break;
            }
            case SaveFileMode.CONTINUE: {
                string filePath = Application.persistentDataPath + "/save_data_" + saveFile + ".dat";
                FileStream file;

                if (File.Exists(filePath) == false) {
                    return;
                }

                AdjustButtonsParentPosition(saveFile);
                deleteButton.SetActive(true);
                buttonsParent.SetActive(true);

                Utils.saveFile = saveFile;
                
                file = File.OpenRead(filePath);

                BinaryFormatter bf = new BinaryFormatter();
                ArrayList data = (ArrayList)bf.Deserialize(file);
                file.Close();

                Utils.username = (string) data[0];

                break;
            }
            case SaveFileMode.NULL: {
                Debug.LogError("Save menu not entered from New Game or Continue button!");
                break;
            }
        }
    }

    public void ConfirmBeginGame() {
        switch (saveMode) {
            case SaveFileMode.NEW_GAME: {
                NewGame();
                break;
            }
            case SaveFileMode.CONTINUE: {
                LoadGame();
                break;
            }
            case SaveFileMode.NULL: {
                Debug.LogError("Whoops... Confirm button appeared without setting up save file first!");
                break;
            }
        }
    }

    public void CancelCreateNewSave() {
        Utils.saveFile = 0;
        usernameField.gameObject.SetActive(false);
        buttonsParent.gameObject.SetActive(false);
    }

    public void DeleteSaveFile() {
        string filePath = Application.persistentDataPath + "/save_data_" + Utils.saveFile + ".dat";
        FileStream file;

        File.Delete(filePath);
    }

    private void NewGame() {
        Utils.username = usernameField.text;
        Utils.InitGameData();

        string filePath = Application.persistentDataPath + "/save_data_" + Utils.saveFile + ".dat";
        FileStream file;

        file = File.Create(filePath);

        ArrayList data = new ArrayList(1) {
            Utils.username
        };

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();

        SceneManager.LoadScene("SampleScene");
    }

    private void LoadGame() {
        Utils.InitGameData();
        SceneManager.LoadScene("SampleScene");
    }

    private void AdjustButtonsParentPosition(int saveFile) {
        cancelButton.SetActive(false);
        deleteButton.SetActive(false);

        RectTransform buttonsParentRT = (RectTransform)buttonsParent.transform,
                        saveButtonRT = (RectTransform)saveButtons[saveFile - 1].transform;
        
        float yOffset = saveButtonRT.rect.height / 2 + buttonsParentRT.rect.height / 2;
        buttonsParent.transform.localPosition = saveButtons[saveFile - 1].transform.localPosition - new Vector3(0f, yOffset, 0f);
    }
}
