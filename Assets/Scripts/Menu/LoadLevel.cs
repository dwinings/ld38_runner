﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class LoadLevel : MonoBehaviour {

  	// Use this for initialization
  	void Start () {
  		
  	}
  	
  	// Update is called once per frame
  	void Update () {
  		
  	}

    public void LoadLevel1() {
      SceneManager.LoadScene(1);
    }
  }
}