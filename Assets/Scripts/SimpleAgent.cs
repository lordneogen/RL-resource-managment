using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class MainAgent : Agent
{
    public int day_start;
    public int day_end;
    public int day;
    public List<IUseTick> builds=new List<IUseTick>();
    public float batteryRes=100f;
    
    //
    public override void OnEpisodeBegin()
    {
        day = 0;
    }

    // Collect observations of the environment
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(day_start);
        sensor.AddObservation(day_end);
        sensor.AddObservation(batteryRes);
        sensor.AddObservation(builds.Count);
        sensor.AddObservation(day);
    }

    // Actions to control the agent (called every step)
    public override void OnActionReceived(ActionBuffers actions)
    {
        // // Actions: [0] - Move, [1] - Rotate
        // float move = actions.ContinuousActions[0];
        // float rotate = actions.ContinuousActions[1];
        
        //
        // // Move and rotate the agent
        // transform.Rotate(Vector3.up, rotationSpeed * rotate * Time.deltaTime);
        // transform.Translate(Vector3.forward * moveSpeed * move * Time.deltaTime);
        //
        // // Reward the agent based on distance to the target
        // float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        //
        // // Give a positive reward if the agent is close to the target
        // if (distanceToTarget < 1.5f)
        // {
        //     SetReward(1.0f);
        //     EndEpisode();
        // }
        // // Penalize the agent if it moves out of bounds
        // else if (transform.localPosition.y < 0)
        // {
        //     SetReward(-1.0f);
        //     EndEpisode();
        // }
    }

    // Manual control for testing (optional)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical"); // Forward/backward
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // Rotate
    }
}
