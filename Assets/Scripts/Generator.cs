using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;

public class Generator:MonoBehaviour
{
    public AgentWithGenerator agentWithGenerator;

    public static Generator Instance;

    private void Awake()
    {
        Generator.Instance = this;
    }
}