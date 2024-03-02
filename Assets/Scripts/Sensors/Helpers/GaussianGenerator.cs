using UnityEngine;
using System;

public class GaussianGenerator
{
    System.Random random;
    double mean;
    double standard_deviation;

    public GaussianGenerator() {
        random = new System.Random();
        mean = 0.0;
        standard_deviation = 1.0;
    }

    public GaussianGenerator(double mean_param, double standard_deviation_param) {
        random = new System.Random();
        mean = mean_param;
        standard_deviation = standard_deviation_param;
    }

    // Reference: Box Muller transform for generating gaussian distributed variables
    // https://stackoverflow.com/questions/218060/random-gaussian-variables

    public double next() {
        // Generates gaussian random using box muller transformation
        // error = sd * random(0,1) * sqrt(-2 * log(random(0,1)) + mean)

        //return ((standard_deviation * random.NextDouble() * Math.Sqrt(-2.0 * Math.Log(random.NextDouble()))) + mean);


        // Box Muller Transform

        //Random rand = new Random(); //reuse this if you are generating many
        double u1 = 1.0-random.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0-random.NextDouble();


        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *  Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = mean + standard_deviation * randStdNormal;

        return randNormal;
        
    }
    public double next(double mean_param, double standard_deviation_param) {
        //return ((standard_deviation_param * random.NextDouble() * Math.Sqrt(-2.0 * Math.Log(random.NextDouble()))) + mean_param);

        // Box Muller Transform

        //Random rand = new Random(); //reuse this if you are generating many
        double u1 = 1.0-random.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0-random.NextDouble();


        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *  Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = mean_param + standard_deviation_param * randStdNormal;

        return randNormal;
    }

    public double add_noise_scale(double original_value) {
        // Decimal of percentage of noise to add, next = noise gaussian
        return (original_value*(1.0 + next()));
    }

    public Vector3 add_noise_scale(Vector3 original_values) {
        return (new Vector3{
            x = (float)(original_values.x+(original_values.x*next())),
            y = (float)(original_values.y+(original_values.y*next())),
            z = (float)(original_values.z+(original_values.z*next()))
        });
    }

    public Quaternion add_noise_scale(Quaternion original_values) {
            return (new Quaternion{
                x = (float)(original_values.x*(1.0d+next())),
                y = (float)(original_values.y*(1.0d+next())),
                z = (float)(original_values.z*(1.0d+next())),
                w = (float)(original_values.w*(1.0d+next()))
            });
    }

    public double add_noise(double original_value) {
        return (original_value + next());
    }
}