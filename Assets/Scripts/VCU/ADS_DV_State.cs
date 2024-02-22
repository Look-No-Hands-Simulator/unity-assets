using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Robotics.Core;
using TMPro;

// Custom namespace msgs
using RosMessageTypes.AdsDv;

public class ADS_DV_State : MonoBehaviour {

	public Button stateButton;
	public Button ebsButton;
	public Button goButton;

	public GameObject carObject;
	public WheelCollider fl_wheel_collider;
    public WheelCollider fr_wheel_collider;
    public WheelCollider bl_wheel_collider;
    public WheelCollider br_wheel_collider;

    private ASSI_Manager assi_manager;
    public int assi_material_index = 5;


	private DateTime stateChangeTimestamp;
	private int front_axle_torque_request;
	private int rear_axle_torque_request;
	private short steer_angle_request;
	private short actual_steer_angle;
	private bool brake_plausibility_fault;
	private bool bms_fault;

	//private byte assi_light;
	private const byte ASSI_LIGHT_OFF = 0;
	private const byte ASSI_LIGHT_YELLOW_FLASHING = 1;
	private const byte ASSI_LIGHT_YELLOW_CONTINUOUS = 2;
	private const byte ASSI_LIGHT_BLUE_FLASHING = 3;
	private const byte ASSI_LIGHT_BLUE_CONTINUOUS = 4;

	/// VCU2AI

	// EBS status (additional internal)
	private byte ebs_state;
	private const byte EBS_STATE_ARMED = 0;
	private const byte EBS_STATE_UNAVAILABLE = 1;

	public readonly string[] ebs_state_array = {"EBS_STATE_ARMED","EBS_STATE_UNAVAILABLE"};

	//  Handshake
    private bool handshake;

    //  Shutdown request to AI
    private bool shutdown_request;

    //  Autonomous System switch status
    private bool as_switch_status;

    //  Tractive System switch status
    private bool ts_switch_status;

    //  Autonomous System "Go" signal
    private bool go_signal;

    //  State of the steering system [0 - 3]
    private byte steering_status;
    private const byte STEERING_STATUS_OFF = 0;
    private const byte STEERING_STATUS_ACTIVE = 1;

    //  State of the Autonomous System [0 - 7]
    private byte as_state;
    public const byte AS_STATE_AS_OFF = 0;
    public const byte AS_STATE_AS_READY = 1;
    public const byte AS_STATE_AS_DRIVING = 2;
    public const byte AS_STATE_EMERGENCY_BRAKE = 3;
    public const byte AS_STATE_AS_FINISHED = 4;

    private readonly string[] state_collection = {"AS_STATE_AS_OFF","AS_STATE_AS_READY","AS_STATE_AS_DRIVING","AS_STATE_EMERGENCY_BRAKE","AS_STATE_AS_FINISHED"};

    //  State of the Mission Indicator [0 - 7]
    private byte ami_state;
    private const byte AMI_STATE_NOT_SELECTED = 0;
    private const byte AMI_STATE_ACCELERATION = 1;
    private const byte AMI_STATE_SKIDPAD = 2;
    private const byte AMI_STATE_AUTOCROSS = 3;
    private const byte AMI_STATE_TRACK_DRIVE = 4;
    private const byte AMI_STATE_BRAKE_TEST = 5;
    private const byte AMI_STATE_INSPECTION = 6;

    //  System fault status
    private bool fault_status;

    //  System warning status
    private bool warning_status;

    //  High traction battery temperature warning
    private bool warn_batt_temp_high;

    //  Low traction battery SOC warning
    private bool warn_batt_soc_low;

    //  AI system E-stop request
    private bool ai_estop_request;

    //  HVIL open-circuit fault
    private bool hvil_open_fault;

    //  HVIL short-circuit fault
    private bool hvil_short_fault;

    //  EBS fault
    private bool ebs_fault;

    //  Offboard charger fault
    private bool offboard_charger_fault;

    //  AI CAN communications fault
    private bool ai_comms_lost;

    //  Braking in Autonomous Driving mode fault
    private bool autonomous_braking_fault;

    //  Autonomous mission status fault
    private bool mission_status_fault;
    private bool reserved_1;
    private bool reserved_2;

    /// AI2VCU

    //  Handshake
    //private bool handshake;

    //  Shutdown request to VCU
    private bool estop_request;

    //  Autonomous mission status [0 - 3]
    private byte mission_status;
    private const byte MISSION_STATUS_NOT_SELECTED = 0;
    private const byte MISSION_STATUS_SELECTED = 1;
    private const byte MISSION_STATUS_RUNNING = 2;
    private const byte MISSION_STATUS_FINISHED = 3;

    //  Requested vehicle direction [0 - 3]
    private byte direction_request;
    private const byte DIRECTION_REQUEST_NEUTRAL = 0;
    private const byte DIRECTION_REQUEST_FORWARD = 1;
    private const byte DIRECTION_REQUEST_REVERSE = 2;

    //  Lap counter [0 - 15]
    private byte lap_counter;

    //  Number of cones detected [0 - 255]
    private byte cones_count_actual;

    //  Total number of cones detected [0 - 65535]
    private ushort cones_count_all;

    //  Actual vehicle speed [0 - 255] km/h
    private byte veh_speed_actual_kmh;

    //  Demanded vehicle speed [0 - 255] km/h
    private byte veh_speed_demand_kmh;


    public void Start() {

    	// Lights manager
    	assi_manager = new ASSI_Manager(carObject, assi_material_index);

    	front_axle_torque_request = 0;
    	rear_axle_torque_request = 0;
    	steer_angle_request = (short)0.0;
    	actual_steer_angle = (short)0.0;

    	//assi_light = ASSI_LIGHT_OFF;
    	assi_manager.SetState(ASSI_LIGHT_OFF);

    	brake_plausibility_fault = false;
    	bms_fault = false;

    	/// VCU2AI
    	// TODO: check the handshake
    	handshake = true;
    	shutdown_request = false;
    	as_switch_status = false;
    	ts_switch_status = false;
    	go_signal = false;
    	steering_status = STEERING_STATUS_OFF;
    	SetAsState(AS_STATE_AS_OFF); // set as_state
    	ami_state = AMI_STATE_NOT_SELECTED;
    	fault_status = false;
    	warning_status = false;
    	warn_batt_temp_high = false;
    	warn_batt_soc_low = false;
    	ai_estop_request = false;
    	hvil_open_fault = false;
    	hvil_short_fault = false;
    	ebs_fault = false;
    	offboard_charger_fault = false;
    	ai_comms_lost = false;
    	autonomous_braking_fault = false;
    	mission_status_fault = false;
    	reserved_1 = false;
    	reserved_2 = false;

    	///AI2VCU
    	// Unsure about this estop request if it is duplicate
    	estop_request = false;
    	mission_status = MISSION_STATUS_NOT_SELECTED;
    	direction_request = DIRECTION_REQUEST_NEUTRAL;
    	lap_counter = 0;
    	cones_count_actual = 0;
    	cones_count_all = 0;
    	veh_speed_actual_kmh = 0;
    	veh_speed_demand_kmh = 0;



    	assi_manager.Start();

    	//as_switch_status = false;
    	//ts_switch_status = true;
    	SetEBSState(EBS_STATE_ARMED);
    	mission_status = MISSION_STATUS_SELECTED;


    }

    public void Update() {

    	UpdateState();
    	assi_manager.Update();

    }

    private void SetAsState(byte newState) {
    	as_state = newState;

    	stateButton.GetComponentInChildren<TextMeshProUGUI>().text = state_collection[newState];

    	// Get Unix time, how long since Jan 1st 1970?
    	// start a timer upon state change
        stateChangeTimestamp = DateTime.Now;
    }

    public void UpdateState() {


    	switch(as_state) {

    		case AS_STATE_AS_OFF:

    			if (as_switch_status == true && ts_switch_status == true && ebs_state == EBS_STATE_ARMED && mission_status == MISSION_STATUS_SELECTED) {

    				SetAsState(AS_STATE_AS_READY);
    				assi_manager.SetState(ASSI_LIGHT_YELLOW_CONTINUOUS);

    			}

    			break;

    		case AS_STATE_AS_READY:

    			if (as_switch_status == false) {

    				SetAsState(AS_STATE_AS_OFF);
    				assi_manager.SetState(ASSI_LIGHT_OFF);

    			} else if (shutdown_request == true) {

    				// the SDC open
    				SetAsState(AS_STATE_AS_OFF);
    				assi_manager.SetState(ASSI_LIGHT_OFF);

    			} else if (TimeElapsedInCurrentState(5D) && front_axle_torque_request == 0 && rear_axle_torque_request == 0 && 
    			steer_angle_request == 0 && Math.Abs(actual_steer_angle) < 5 && direction_request == DIRECTION_REQUEST_NEUTRAL
    			&& go_signal == true ) {

    				// Check this go signal condition in manual
    				SetAsState(AS_STATE_AS_DRIVING);
    				// ASSI light
    				assi_manager.SetState(ASSI_LIGHT_YELLOW_FLASHING);
    			}

    			break;

    		case AS_STATE_AS_DRIVING:

    			if (mission_status == MISSION_STATUS_FINISHED && fl_wheel_collider.rpm < 10D && fr_wheel_collider.rpm < 10D && bl_wheel_collider.rpm < 10D && 
    				br_wheel_collider.rpm < 10D) {

    				SetAsState(AS_STATE_AS_FINISHED);
    				assi_manager.SetState(ASSI_LIGHT_BLUE_CONTINUOUS);

    			} else if (shutdown_request == true || as_switch_status == false || go_signal == false || mission_status_fault == true ||
    				autonomous_braking_fault == true || brake_plausibility_fault == true || ai_estop_request == true || ai_comms_lost == true
    				|| bms_fault == true || ebs_state == EBS_STATE_UNAVAILABLE ) {

    				SetAsState(AS_STATE_EMERGENCY_BRAKE);
    				assi_manager.SetState(ASSI_LIGHT_BLUE_FLASHING);
    			}

    			break;

    		case AS_STATE_EMERGENCY_BRAKE:

    			if (TimeElapsedInCurrentState(15D) && as_switch_status == false) {

    				SetAsState(AS_STATE_AS_OFF);
    				assi_manager.SetState(ASSI_LIGHT_OFF);
    			}

    			break;

    		case AS_STATE_AS_FINISHED:
    			if (shutdown_request == true) {

    				//SDC is open
    				SetAsState(AS_STATE_EMERGENCY_BRAKE);
    				assi_manager.SetState(ASSI_LIGHT_BLUE_FLASHING);

    			} else if (as_switch_status == false) {

    				SetAsState(AS_STATE_AS_OFF);
    				assi_manager.SetState(ASSI_LIGHT_OFF);
    			}

    			break;
    	}
    }

    private bool TimeElapsedInCurrentState(double seconds) {

    	DateTime endTime = DateTime.Now;
    	TimeSpan elapsed = endTime - stateChangeTimestamp;

    	return elapsed.TotalSeconds >= seconds;

    }

    public void SwitchASState() {

    	if (as_switch_status == true) {
    		as_switch_status = false;
    	} else {
    		as_switch_status = true;
    	}


    }

    public void SwitchTSState() {
    	
    	if (ts_switch_status == true) {
    		ts_switch_status = false;
    	} else {
    		ts_switch_status = true;
    	}

    }

    public void SwitchGoSignal() {
    	
    	if (go_signal == true) {
    		go_signal = false;
    		goButton.GetComponent<Image>().color = Color.yellow;
    	} else {
    		go_signal = true;
    		goButton.GetComponent<Image>().color = Color.green;
    	}

    }

    public void SetEBSState(byte newEBSState) {

    	ebs_state = newEBSState;
    	ebsButton.GetComponentInChildren<TextMeshProUGUI>().text = ebs_state_array[newEBSState];

    }

    public void ActivateEBS() {

    	if (ebs_state == EBS_STATE_ARMED) {
    		SetEBSState(EBS_STATE_UNAVAILABLE);
    	} else {
    		SetEBSState(EBS_STATE_ARMED);
    	}

    }

    public byte GetAsState() {

    	return as_state;
    }

    public VCU2AIStatusMsg get_vcu2aiStatus_msg() {

    	VCU2AIStatusMsg vcu2ai_msg = new VCU2AIStatusMsg();

    	vcu2ai_msg.handshake = this.handshake;
        vcu2ai_msg.shutdown_request = this.shutdown_request;
        vcu2ai_msg.as_switch_status = this.as_switch_status;
        vcu2ai_msg.ts_switch_status = ts_switch_status;
        vcu2ai_msg.go_signal = false;
        vcu2ai_msg.steering_status = 0;
        vcu2ai_msg.as_state = 0;
        vcu2ai_msg.ami_state = 0;
        vcu2ai_msg.fault_status = false;
        vcu2ai_msg.warning_status = false;
        vcu2ai_msg.warn_batt_temp_high = false;
        vcu2ai_msg.warn_batt_soc_low = false;
        vcu2ai_msg.ai_estop_request = false;
        vcu2ai_msg.hvil_open_fault = false;
        vcu2ai_msg.hvil_short_fault = false;
        vcu2ai_msg.ebs_fault = false;
        vcu2ai_msg.offboard_charger_fault = false;
        vcu2ai_msg.ai_comms_lost = false;
        vcu2ai_msg.autonomous_braking_fault = false;
        vcu2ai_msg.mission_status_fault = false;
        vcu2ai_msg.reserved_1 = false;
        vcu2ai_msg.reserved_2 = false;

    }


}