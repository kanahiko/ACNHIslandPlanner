using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveUI : MonoBehaviour
{
    public List<SaveSlotUI> slots;
    public Color activeSlot = Color.blue;
    public Color inactiveSlot = Color.gray;

    public Canvas selfCanvas;
    public Canvas alertPopUp;

    private Action nextAction;

    public Action<bool> Pause;

    private void Awake()
    {
        SaveSystem.FindSlots();
        SaveSystem.activateSlot = ActivateSlot;

        for (int i = 0; i < SaveSystem.saveSlots.Length; i++)
        {
            int index = i;
            MakeSlot(index);
            
            slots[i].saveButton.onClick.AddListener((() => SaveButton(index)));
            slots[i].newButton.onClick.AddListener((() => NewButton(index)));
            slots[i].deleteButton.onClick.AddListener((() => DeleteButton(index)));
            slots[i].loadButton.onClick.AddListener((() => LoadButton(index)));
        }
    }

    public void DoAction()
    {
        nextAction?.Invoke();
        nextAction = null;
    }

    public void Cancel()
    {
        nextAction = null;
    }

    void LoadButton(int index)
    {
        if (MapHolder.isDirty)
        {
            alertPopUp.enabled = true;
            nextAction = () => LoadForReal(index);
        }
        else
        {
            LoadForReal(index);
        }
    }

    void LoadForReal(int index)
    {
        SaveSystem.Load(index);
        UnPause();
    }

    void SaveButton(int index)
    {
        if (SaveSystem.currentSlot != -1 && SaveSystem.currentSlot != index && SaveSystem.saveSlots[index].isWritten)
        {
            alertPopUp.enabled = true;
            nextAction = () => SaveForReal(index);
        }
        else
        {
            SaveForReal(index);
        }
    }

    void SaveForReal(int index)
    {
        SaveSystem.Save(index);
        MakeSlot(index);
        UnPause();
    }

    void NewButton(int index)
    {
        if (SaveSystem.currentSlot != -1 ||  MapHolder.isDirty)
        {
            alertPopUp.enabled = true;
            nextAction = () => NewButtonForReal(index);
        }
        else
        {
            NewButtonForReal(index);
        }
    }
    
    void NewButtonForReal(int index)
    {
        SaveSystem.New(index);
        MakeSlot(index);
        UnPause();
    }

    void DeleteButton(int index)
    {
        if (index != -1)
        {
            alertPopUp.enabled = true;
            nextAction = () => DeleteButtonForReal(index);
        }
    }
    
    void DeleteButtonForReal(int index)
    {
        SaveSystem.Delete(index);
        MakeSlot(index);
    }

    public void UnPause()
    {
        selfCanvas.enabled = false;
        Pause?.Invoke(false);
    }

    public void ActivateSlot(int index, bool activate)
    {
        if (activate)
        {
            slots[index].background.color = activeSlot;
        }
        else
        {
            slots[index].background.color = inactiveSlot;
        }
    }

    void MakeSlot(int index)
    {
        if (SaveSystem.saveSlots[index].isWritten)
        {
            slots[index].dateText.text = SaveSystem.saveSlots[index].date;
            slots[index].dateText.enabled = true;

            slots[index].saveButtonImage.enabled = true;
            slots[index].saveButtonIcon.enabled = true;
            slots[index].newButtonImage.enabled = false;
            slots[index].newButtonIcon.enabled = false;

            slots[index].deleteButtonImage.enabled = true;
            slots[index].deleteButtonIcon.enabled = true;
            slots[index].loadButtonImage.enabled = true;
            slots[index].loadButtonIcon.enabled = true;
        }
        else
        {
            slots[index].dateText.enabled = false;

            slots[index].saveButtonImage.enabled = true;
            slots[index].saveButtonIcon.enabled = true;
            slots[index].newButtonImage.enabled = true;
            slots[index].newButtonIcon.enabled = true;

            slots[index].deleteButtonImage.enabled = false;
            slots[index].deleteButtonIcon.enabled = false;
            slots[index].loadButtonImage.enabled = false;
            slots[index].loadButtonIcon.enabled = false;
        }
    }
}