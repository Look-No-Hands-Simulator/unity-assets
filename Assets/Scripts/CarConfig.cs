using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;


public class CarConfig : MonoBehaviour {

/// Drive System
public const int WHEELSPEEDS_TEETH = 20; 
public const float WHEEL_DIAMETER = 0.49f; // M
public const double WHEEL_CIRCUMFERENCE = WHEEL_DIAMETER * Math.PI; 
public const int MAX_MOTOR_SPEED = 4000; // RPM
public const float PEAK_MOTOR_TORQUE = 55.7f; // Nm
public const float PEAK_MOTOR_POWER = 17.0f; // kW 48V
public const float CONTINUOUS_MOTOR_TORQUE = 27.2f; // Nm 
public const float CONTINIOUS_MOTOR_POWER = 8.8f; // kW
public const float BELT_DRIVE_RATIO = 3.5f;
public const int PEAK_MOTOR_CURRENT = 400; // A
public const string DRIVE_MOTORS = "Saietta 119R-68(X2)";
public const string MOTOR_CONTROLLERS = "Sevcon Gen4 DC (X2)";
public const int MIN_OPERATING_VOLTAGE_RANGE = 31; // V
public const int MAX_OPERATING_VOLTAGE_RANGE = 90; // V
public const int PEAK_CURRENT_RATING = 550; // A
public const int MAX_TORQUE = 195;
public const float WHEELBASE = 1.2f; 


        // #Params
        // self.declare_parameter('wheel_diameter', 0.49) #0.49 ADS-DV value
        // self.declare_parameter('wheel_teeth_count', 20)
        // self.declare_parameter('max_rpm', 4000)
        // self.declare_parameter('max_trq', 195)
        // # self.declare_parameter('wheelbase', 1.54)
        // self.declare_parameter('wheelbase', 1.2) 
        // self.declare_parameter('origin_2_r_axle', 1.26) # TODO: measure on ADS
        // self.declare_parameter('steer_gain', 1.2)

        // self.declare_parameter('rpm_delimiter', False) # TODO
        // self.declare_parameter('delimiter_max_rpm', 250)
        // self.declare_parameter('invert_steer', False)
        // self.declare_parameter('lookahead', 3.5)

/// Steering System

/// Traction Battery

/// Sensors

}