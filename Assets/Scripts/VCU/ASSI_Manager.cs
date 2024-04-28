using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Robotics.Core;

public class ASSI_Manager {

	private byte assi_light;
	public const byte ASSI_LIGHT_OFF = 0;
	public const byte ASSI_LIGHT_YELLOW_FLASHING = 1;
	public const byte ASSI_LIGHT_YELLOW_CONTINUOUS = 2;
	public const byte ASSI_LIGHT_BLUE_FLASHING = 3;
	public const byte ASSI_LIGHT_BLUE_CONTINUOUS = 4;

	private GameObject car;
	private int assi_material_index = 5;
	private Material assi_material;
	private Material[] car_materials;

	private float update_interval;
	private float time_elapsed = 0.0f;

	private bool flash_status;

	public void Start() {

		car_materials = car.GetComponent<Renderer>().materials;
		assi_material = car_materials[assi_material_index];

		// foreach (Material mat in car_materials) {
		// 	Debug.Log("material name: " + mat.name);
		// }
	}

	public ASSI_Manager(GameObject carParam, int materialIndexParam) {

		car = carParam;
		assi_material_index = materialIndexParam;

		flash_status = true;
		update_interval = 0.5f;
		assi_light = ASSI_LIGHT_OFF;
		
	}

	public byte GetAssiLightStatus() {

		return assi_light;
	}

	public void SetState(byte NewState) {

		assi_light = NewState;

	}

	public void Update() {

		time_elapsed += Time.deltaTime;
        if (time_elapsed > update_interval) {

        	switch(assi_light) {
        		case ASSI_LIGHT_OFF:

        			assi_material.color = Color.white;

        			break;

        		case ASSI_LIGHT_YELLOW_FLASHING:

        			flash_status = !flash_status;
        			if (flash_status) {
        				assi_material.color = Color.yellow;
        			} else {
        				assi_material.color = Color.white;
        			}

        			break;

        		case ASSI_LIGHT_YELLOW_CONTINUOUS:

        			assi_material.color = Color.yellow;

        			break;

        		case ASSI_LIGHT_BLUE_FLASHING:

        			flash_status = !flash_status;
        			if (flash_status) {
        				assi_material.color = Color.blue;
        			} else {
        				assi_material.color = Color.white;
        			}

        			break;

        		case ASSI_LIGHT_BLUE_CONTINUOUS:

        			assi_material.color = Color.blue;

        			break;
        	}


            time_elapsed = 0.0f;
        }
        

	}
}