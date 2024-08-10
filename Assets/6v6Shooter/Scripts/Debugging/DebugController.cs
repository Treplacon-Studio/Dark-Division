using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    [SerializeField] private Color backgroundColor;
    [SerializeField] private PlayerHitIndicator playerHitIndicator;

    private bool showHelp;
    private bool showConsole;
    private string input;
    private Vector2 scroll;

    private List<string> autoSuggestions = new List<string>();
    private int selectedSuggestionIndex = -1;

    public List<object> commandList;
    public static DebugCommand HELP;
    public static DebugCommand RANDOM_DIRECTION_DAMAGE;
    public static DebugCommand<int> ADD_50_GRENADES;
    public static DebugCommand<int> ADD_50_STUNS;
    public static DebugCommand<int> ADD_50_FLASHBANGS;
    public static DebugCommand<int> ADD_50_SMOKES;
    public static DebugCommand<int> ADD_50_THROWINGKNIFES;

    private void Awake()
    {
        AddCommandToList();
    }

    private void AddCommandToList()
    {
        //Follow this style to add a debug command
        HELP = new DebugCommand("HELP", "Show all possible commands", "help", () =>
        {
            showHelp = true;
        });

        RANDOM_DIRECTION_DAMAGE = new DebugCommand("RANDOM_DIRECTION_DAMAGE", "Take damage from a random angle", "random_direction_damage", () =>
        {
            playerHitIndicator.GenerateRandomImpact();
        });

        ADD_50_GRENADES = new DebugCommand<int>("ADD_50_GRENADES", "Add 50 more grenades", "add_50_grenades", (x) =>
        {
            //ADD method call here
        });

        //Make sure to add the debug command here to add it to the list
        commandList = new List<object>
        {
            HELP,
            RANDOM_DIRECTION_DAMAGE,
            ADD_50_GRENADES,
            ADD_50_STUNS,
            ADD_50_FLASHBANGS,
            ADD_50_SMOKES,
            ADD_50_THROWINGKNIFES
        };
    }

    public void OnToggleDebug(InputValue value)
    {
        showConsole= !showConsole;
    }

    public void OnReturn(InputValue value)
    {
        if (showConsole)
        {
            if (autoSuggestions.Count > 0 && selectedSuggestionIndex >= 0)
            {
                input = autoSuggestions[selectedSuggestionIndex];
                autoSuggestions.Clear();
            }
            else
            {
                handleInput();
                input = "";
            }
        }
    }

    public void OnNavigate(InputValue value)
    {
        Vector2 navigation = value.Get<Vector2>();

        if (navigation.y > 0) // Up
        {
            selectedSuggestionIndex = Mathf.Max(selectedSuggestionIndex - 1, 0);
        }
        else if (navigation.y < 0) // Down
        {
            selectedSuggestionIndex = Mathf.Min(selectedSuggestionIndex + 1, autoSuggestions.Count - 1);
        }
    }

    private void OnGUI()
    {
        if (!showConsole) { return; }

        float y = 0f;

        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                string label = $"{command.CommandFormat} - {command.CommandDescription}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = backgroundColor;
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);

        // Update suggestions based on input
        UpdateAutoSuggestions();

        // Show auto-suggestions
        if (autoSuggestions.Count > 0)
        {
            for (int i = 0; i < autoSuggestions.Count; i++)
            {
                GUI.backgroundColor = i == selectedSuggestionIndex ? Color.gray : Color.white;
                GUI.Box(new Rect(10f, y + 35f + (25f * i), Screen.width - 20f, 25f), autoSuggestions[i]);
            }
        }
    }

    private void UpdateAutoSuggestions()
    {
        autoSuggestions.Clear();
        if (string.IsNullOrEmpty(input)) return;

        foreach (DebugCommandBase command in commandList)
        {
            if (command.CommandFormat.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            {
                autoSuggestions.Add(command.CommandFormat);
            }
        }

        // Reset selected suggestion index if the list changes
        selectedSuggestionIndex = -1;
    }

    private void handleInput()
    {
        string[] properties = input.Split(' ');

        for(int i=0; i<commandList.Count; i++)
        {
            DebugCommandBase commandbase = commandList[i] as DebugCommandBase;

            if(input.Contains(commandbase.CommandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
            }
            else if (commandList[i] as DebugCommand<int> != null)
            {
                (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
            }
        }
    }
}
