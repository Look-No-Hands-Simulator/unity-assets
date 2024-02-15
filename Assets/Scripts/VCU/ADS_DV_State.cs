using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ADS_DV_State {

	private DateTime stateChangeTimestamp;
	private int front_axle_torque_request;
	private int rear_axle_torque_request;
	private short steer_angle_request;
	private short actual_steer_angle;

	/// VCU2AI

	// EBS status (additional internal)
	private byte ebs_state;
	private const byte EBS_STATE_ARMED = 0;
	private const byte EBS_STATE_UNAVAILABLE = 1;

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
    private const byte AS_STATE_AS_OFF = 1;
    private const byte AS_STATE_AS_READY = 2;
    private const byte AS_STATE_AS_DRIVING = 3;
    private const byte AS_STATE_EMERGENCY_BRAKE = 4;
    private const byte AS_STATE_AS_FINISHED = 5;

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

    public ADS_DV_State() {
    	front_axle_torque_request = 0;
    	rear_axle_torque_request = 0;
    	steer_angle_request = 0.0;
    	actual_steer_angle = 0.0;

    	/// VCU2AI
    	handshake = false;
    	shutdown_request = false;
    	as_switch_status = false;
    	ts_switch_status = false;
    	go_signal = false;
    	steering_status = STEERING_STATUS_OFF;
    	setAsState(AS_STATE_AS_OFF); // set as_state
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

    }

    private void SetAsState(byte newState) {
    	as_state = newState;

    	// Get Unix time, how long since Jan 1st 1970?
    	// start a timer upon state change
        stateChangeTimestamp = Clock.time;
    }

    public void UpdateState() {


    	switch(as_state) {
    		case AS_STATE_AS_OFF:
    			if (as_switch_status == true && ts_switch_status == true && ebs_state == EBS_STATE_ARMED && mission_status == MISSION_STATUS_SELECTED) {
    				setAsState(AS_STATE_AS_READY);

    			}
    			break;
    		case AS_STATE_AS_READY:
    			if (as_switch_status == false) {
    				setAsState(AS_STATE_AS_OFF);
    			} else if (shutdown_request == true) {
    				// the SDC open
    				setASState(AS_STATE_AS_OFF);
    			} else if (TimeElapsedInCurrentState(5D) && front_axle_torque_request == 0 && rear_axle_torque_request == 0 && 
    			steer_angle_request == 0 && Math.Abs(actual_steer_angle) < 5 && direction_request == DIRECTION_REQUEST_NEUTRAL
    			&& go_signal == true ) {
    				// Check this go signal condition in manual
    				setAsState(AS_STATE_AS_DRIVING);

    			}
    			break;
    		case AS_STATE_AS_DRIVING;
    			break;
    		case AS_STATE_EMERGENCY_BRAKE;
    			break;
    		case AS_STATE_AS_FINISHED;
    			break;
    	}
    }

    private bool TimeElapsedInCurrentState(double seconds) {

    	DateTime endTime = Clock.time;
    	TimeSpan elapsed = endTime - stateChangeTimestamp;

    	return elapsed.TotalSeconds >= seconds;

    }


}