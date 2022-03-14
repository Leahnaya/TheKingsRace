using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingTooltips : MonoBehaviour
{
    
    ////Weather Wheel Variables
    public GameObject curWeatherSpriteObj;
    public Text curWeatherText;
    public Image curWeatherImage;
    float curSpriteRotation;

    string rainExplanation = "Increases the players acceleration & decreases the players traction.";
    string snowExplanation = "Increases the players traction & decreases the players acceleration.";
    string windExplanation = "Pushes the player in a random direction altering their movement.";
    string fogExplanation = "Obscures the players vision.";
    string defaultWeatherExplanation;

    public Sprite rain;
    public Sprite snow;
    public Sprite wind;
    public Sprite fog;
    ////

    ////Placeables Variables
    public GameObject curPlaceablesSpriteObj;
    public Text curPlaceablesText;
    public Image curPlaceablesImage;

    string blockExplanation = "Place a block on the grid to obstruct a players movement.";
    string thunderExplanation = "Places Thunder Above both players forcing them to the ground if they stay aerial for too long.";
    string slimeExplanation = "Click and hold the mouse to place and then choose the direction the slowing slime goes.";
    string hailExplanation = "Click and hold the mouse to adjust the hail size dropping papers";
    string defaultPlaceablesExplanation;

    public Sprite block;
    public Sprite thunder;
    public Sprite slime;
    public Sprite hail;


    void Start(){
        defaultWeatherExplanation = curWeatherText.text;
        defaultPlaceablesExplanation = curPlaceablesText.text;
        curWeatherSpriteObj.SetActive(false);
        curPlaceablesSpriteObj.SetActive(false);
    }

    ////Weather Wheel Functions
    public void OnEnterRain(){
        curWeatherSpriteObj.SetActive(true);
        curWeatherSpriteObj.transform.localEulerAngles = new Vector3(0, 0, 135);
        curWeatherImage.sprite = rain;
        curWeatherText.text = rainExplanation;
    }

    public void OnEnterSnow(){
        curWeatherSpriteObj.SetActive(true);
        curWeatherSpriteObj.transform.localEulerAngles = new Vector3(0, 0, -135);
        curWeatherImage.sprite = snow;
        curWeatherText.text = snowExplanation;
    }

    public void OnEnterWind(){
        curWeatherSpriteObj.SetActive(true);
        curWeatherSpriteObj.transform.localEulerAngles = new Vector3(0, 0, -45);
        curWeatherImage.sprite = wind;
        curWeatherText.text = windExplanation;
    }

    public void OnEnterFog(){
        curWeatherSpriteObj.SetActive(true);
        curWeatherSpriteObj.transform.localEulerAngles = new Vector3(0, 0, 45);
        curWeatherImage.sprite = fog;
        curWeatherText.text = fogExplanation;
    }

    public void OnExitWeather(){
        curWeatherSpriteObj.SetActive(false);
        curWeatherText.text = defaultWeatherExplanation;
    }
    ////

    ////Placeables Functions
    public void OnEnterBlock(){
        curPlaceablesSpriteObj.SetActive(true);
        curPlaceablesImage.sprite = block;
        curPlaceablesText.text = blockExplanation;
    }

    public void OnEnterThunder(){
        curPlaceablesSpriteObj.SetActive(true);
        curPlaceablesImage.sprite = thunder;
        curPlaceablesText.text = thunderExplanation;
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
