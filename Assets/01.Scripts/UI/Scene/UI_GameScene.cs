using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum Images
    {
        BackGround,
        Stage_Slider
    }
    enum Buttons
    {
        Setting_Button,
        Sound_Button,
        Vibe_Button,
        RV_Player_Speed_Button,
        Buy_Player_Speed_Button,
        RV_Player_Capacity_Button,
        Buy_Player_Capacity_Button,
        RV_Player_Income_Button,
        Buy_Player_Income_Button,

        RV_Staff_Speed_Button,
        Buy_Staff_Speed_Button,
        RV_Staff_Capacity_Button,
        Buy_Staff_Capacity_Button,
        RV_Staff_Hire_Button,
        Buy_Staff_Hire_Button,
        PlayerHR_Close_Button,
        StaffHR_Close_Button

    }
    enum Texts
    {
        Money_Text,
        StageGuage_Text,

        PlayerSpeedPriceText,
        PlayerCapacityPriceText,
        PlayerIncomePriceText,
        StaffSpeedPriceText,
        StaffCapacityPriceText,
        StaffHirePriceText

    }

    enum GameObjects
    {
        Jerry,
        Base_Panel,
        Setting_Panel,
        PlayerHR_Panel,
        Player_Speed_Level_Group,
        Player_Capacity_Level_Group,
        Player_Income_Level_Group,

        StaffHR_Panel,
        Staff_Speed_Level_Group,
        Staff_Capacity_Level_Group,
        Staff_Hire_Level_Group



    }

    public Image Stage_Slider;
    public Button Setting_Button, Sound_Button, Vibe_Button, RV_Player_Speed_Button, Buy_Player_Speed_Button, RV_Player_Capacity_Button, Buy_Player_Capacity_Button, RV_Staff_Speed_Button, Buy_Staff_Speed_Button, RV_Staff_Capacity_Button, Buy_Staff_Capacity_Button, RV_Staff_Hire_Button, Buy_Staff_Hire_Button, RV_Player_Income_Button, Buy_Player_Income_Button, PlayerHR_Close_Button,
        StaffHR_Close_Button;
    public Text Money_Text, StageGuage_Text, PlayerSpeedPriceText,
        PlayerCapacityPriceText, PlayerIncomePriceText, StaffSpeedPriceText, StaffCapacityPriceText, StaffHirePriceText;
    public GameObject Base_Panel, Setting_Panel,
        PlayerHR_Panel, Player_Speed_Level_Group, Player_Capacity_Level_Group, Player_Income_Level_Group,
        StaffHR_Panel, Staff_Speed_Level_Group, Staff_Capacity_Level_Group, Staff_Hire_Level_Group;

    // ============================
    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        base.Init();

        SetButton();
    }

    void SetButton()
    {
        Stage_Slider = GetImage(Images.Stage_Slider);

        Setting_Button = GetButton(Buttons.Setting_Button);
        Sound_Button = GetButton(Buttons.Sound_Button);
        Vibe_Button = GetButton(Buttons.Vibe_Button);
        RV_Player_Speed_Button = GetButton(Buttons.RV_Player_Speed_Button);
        Buy_Player_Speed_Button = GetButton(Buttons.Buy_Player_Speed_Button);
        RV_Player_Capacity_Button = GetButton(Buttons.RV_Player_Capacity_Button);
        Buy_Player_Capacity_Button = GetButton(Buttons.Buy_Player_Capacity_Button);
        RV_Player_Income_Button = GetButton(Buttons.RV_Player_Income_Button);
        Buy_Player_Income_Button = GetButton(Buttons.Buy_Player_Income_Button);

        RV_Staff_Speed_Button = GetButton(Buttons.RV_Staff_Speed_Button);
        Buy_Staff_Speed_Button = GetButton(Buttons.Buy_Staff_Speed_Button);
        RV_Staff_Capacity_Button = GetButton(Buttons.RV_Staff_Capacity_Button);
        Buy_Staff_Capacity_Button = GetButton(Buttons.Buy_Staff_Capacity_Button);
        RV_Staff_Hire_Button = GetButton(Buttons.RV_Staff_Hire_Button);
        Buy_Staff_Hire_Button = GetButton(Buttons.Buy_Staff_Hire_Button);
        PlayerHR_Close_Button = GetButton(Buttons.PlayerHR_Close_Button);
        StaffHR_Close_Button = GetButton(Buttons.StaffHR_Close_Button);



        Money_Text = GetText(Texts.Money_Text);
        StageGuage_Text = GetText(Texts.StageGuage_Text);

        PlayerSpeedPriceText = GetText(Texts.PlayerSpeedPriceText);
        PlayerCapacityPriceText = GetText(Texts.PlayerCapacityPriceText);
        PlayerIncomePriceText = GetText(Texts.PlayerIncomePriceText);
        StaffSpeedPriceText = GetText(Texts.StaffSpeedPriceText);
        StaffCapacityPriceText = GetText(Texts.StaffCapacityPriceText);
        StaffHirePriceText = GetText(Texts.StaffHirePriceText);



        Base_Panel = GetObject(GameObjects.Base_Panel);
        Setting_Panel = GetObject(GameObjects.Setting_Panel);

        PlayerHR_Panel = GetObject(GameObjects.PlayerHR_Panel);
        Player_Speed_Level_Group = GetObject(GameObjects.Player_Speed_Level_Group);
        Player_Capacity_Level_Group = GetObject(GameObjects.Player_Capacity_Level_Group);
        Player_Income_Level_Group = GetObject(GameObjects.Player_Income_Level_Group);

        StaffHR_Panel = GetObject(GameObjects.StaffHR_Panel);
        Staff_Speed_Level_Group = GetObject(GameObjects.Staff_Speed_Level_Group);
        Staff_Capacity_Level_Group = GetObject(GameObjects.Staff_Capacity_Level_Group);
        Staff_Hire_Level_Group = GetObject(GameObjects.Staff_Hire_Level_Group);


        // ========= add button event

        Setting_Button.AddButtonEvent(() => Setting_Panel.SetActive(!Setting_Panel.activeSelf));
        Sound_Button.AddButtonEvent(() =>
        {
            Managers.Data.UseSound = !Managers.Data.UseSound;
            Sound_Button.transform.GetChild(1).gameObject.SetActive(!Managers.Data.UseSound);
        });

        Vibe_Button.AddButtonEvent(() =>
        {
            Managers.Data.UseHaptic = !Managers.Data.UseHaptic;
            Vibe_Button.transform.GetChild(1).gameObject.SetActive(!Managers.Data.UseHaptic);
        });

        RV_Player_Speed_Button.AddButtonEvent(() => { });
        Buy_Player_Speed_Button.AddButtonEvent(() =>
        {

            Managers.Game._stagemanager.LevelUpgrade(0);

        });

        RV_Player_Speed_Button.AddButtonEvent(() => { });
        Buy_Player_Speed_Button.AddButtonEvent(() => { Managers.Game._stagemanager.LevelUpgrade(0); });
        RV_Player_Capacity_Button.AddButtonEvent(() => { });
        Buy_Player_Capacity_Button.AddButtonEvent(() => Managers.Game._stagemanager.LevelUpgrade(1));
        RV_Player_Income_Button.AddButtonEvent(() => { });
        Buy_Player_Income_Button.AddButtonEvent(() => Managers.Game._stagemanager.LevelUpgrade(2));

        RV_Staff_Speed_Button.AddButtonEvent(() => { });
        Buy_Staff_Speed_Button.AddButtonEvent(() => { Managers.Game._stagemanager.LevelUpgrade(3); });
        RV_Staff_Capacity_Button.AddButtonEvent(() => { });
        Buy_Staff_Capacity_Button.AddButtonEvent(() => { Managers.Game._stagemanager.LevelUpgrade(4); });
        RV_Staff_Hire_Button.AddButtonEvent(() => { });
        Buy_Staff_Hire_Button.AddButtonEvent(() => { Managers.Game._stagemanager.LevelUpgrade(5); });

        PlayerHR_Close_Button.AddButtonEvent(() => { PlayerHR_Panel.SetActive(false); });
        StaffHR_Close_Button.AddButtonEvent(() => { StaffHR_Panel.SetActive(false); });



    }






}
