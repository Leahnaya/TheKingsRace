using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KingTooltips : MonoBehaviour
{
    
    ////Weather Wheel Variables
    public TextMeshProUGUI curWeatherText;
    public TextMeshProUGUI curWeatherTitle;

    string rainExplanation = "Increases the players acceleration & decreases the players traction.";
    string rainTitle = "Rain";
    string snowExplanation = "Increases the players traction & decreases the players acceleration.";
    string snowTitle = "Snow";
    string windExplanation = "Pushes the player in a random direction altering their movement.";
    string windTitle = "Wind";
    string fogExplanation = "Obscures the players vision.";
    string fogTitle = "Fog";
    string defaultWeatherTitle;
    string defaultWeatherExplanation;
    ////

    ////Placeables Variables
    public GameObject curPlaceablesSpriteObj;
    public TextMeshProUGUI curPlaceablesText;
    public Image curPlaceablesImage;

    string blockExplanation = "Place a block on the grid to obstruct a players movement.";
    string bumperExplanation = "Places a bumper on the grid to obstruct and knock the players back.";
    string slimeExplanation = "Click and hold the mouse to place and then choose the direction the slowing slime goes.";
    string hailExplanation = "Click and hold the mouse to adjust the hail size dropping paper balls.";
    string defaultPlaceablesExplanation;

    public Sprite block;
    public Sprite bumper;
    public Sprite slime;
    public Sprite hail;


    void Start(){
        defaultWeatherTitle = curWeatherTitle.text;
        defaultWeatherExplanation = curWeatherText.text;
        defaultPlaceablesExplanation = curPlaceablesText.text;
        curPlaceablesSpriteObj.SetActive(false);
    }

    ////Weather Wheel Functions
    public void OnEnterRain(){
        curWeatherTitle.text = rainTitle;
        curWeatherText.text = rainExplanation;
    }

    public void OnEnterSnow(){
        curWeatherTitle.text = snowTitle;
        curWeatherText.text = snowExplanation;
    }

    public void OnEnterWind(){
        curWeatherTitle.text = windTitle;
        curWeatherText.text = windExplanation;
    }

    public void OnEnterFog(){
        curWeatherTitle.text = fogTitle;
        curWeatherText.text = fogExplanation;
    }

    public void OnExitWeather(){
        curWeatherTitle.text = defaultWeatherTitle;
        curWeatherText.text = defaultWeatherExplanation;
    }
    ////

    ////Placeables Functions
    public void OnEnterBlock(){
        curPlaceablesSpriteObj.SetActive(true);
        curPlaceablesImage.sprite = block;
        curPlaceablesText.text = blockExplanation;
    }

    public void OnEnterBumper(){
        curPlaceablesSpriteObj.SetActive(true);
        curPlaceablesImage.sprite = bumper;
        curPlaceablesText.text = bumperExplanation;
    }

    public void OnEnterSlime(){
        curPlaceablesSpriteObj.SetActive(true);
        curPlaceablesImage.sprite = slime;
        curPlaceablesText.text = slimeExplanation;
    }

    public void OnEnterHail(){
        curPlaceablesSpriteObj.SetActive(true);
        curPlaceablesImage.sprite = hail;
        curPlaceablesText.text = hailExplanation;
    }

    public void OnExitPlaceables(){
        curPlaceablesSpriteObj.SetActive(false);
        curPlaceablesText.text = defaultPlaceablesExplanation;
    }
    ////
}
