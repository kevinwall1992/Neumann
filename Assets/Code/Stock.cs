﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Stock : MonoBehaviour, HasVariables
{
    public Pile Pile = new Pile();

    List<Request> requests = new List<Request>();

    public List<Variable> Variables
    {
        get
        {
            return new List<Variable>(Pile.Resources.Select(resource => 
                new FunctionVariable(resource.Name, () => Pile.GetVolumeOf(resource))
                    .Stylize(Scene.Main.Style.Variables[resource.Name])));
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Dictionary<Resource, float> resource_yields = new Dictionary<Resource, float>();
        foreach (Resource resource in Pile.Resources)
        {
            float usage_per_second = GetUsagePerSecond(resource);

            if (usage_per_second == 0)
                resource_yields[resource] = 1;
            else
                resource_yields[resource] = Mathf.Min(1, Pile.GetVolumeOf(resource) / 
                                                         (usage_per_second * Time.deltaTime));
        }

        foreach (Request request in requests)
        {
            if (request.UsagePerSecond.Volume > 0)
            {
                request.Yield = request.UsagePerSecond.Resources.Min(resource => resource_yields[resource]);
                request.Disbursement = Pile.TakeOut(request.UsagePerSecond * request.Yield * Time.deltaTime);
            }
        }
    }

    public float GetUsagePerSecond(Resource resource)
    {
        return requests.Sum(request => request.UsagePerSecond.GetVolumeOf(resource));
    }

    public Request MakeRequest(Pile usage_per_second)
    {
        Request request = new Request(this, usage_per_second);
        requests.Add(request);

        return request;
    }


    //Resource units here are [Pile Unit] per second
    public class Request
    {
        Stock stock;

        internal Pile UsagePerSecond { get; set; }

        public Pile Disbursement { get; internal set; }
        public float Yield { get; internal set; }

        internal Request(Stock stock_, Pile usaged_per_second_)
        {
            stock = stock_;
            UsagePerSecond = usaged_per_second_;

            Disbursement = new Pile();
        }

        public void Revoke()
        {
            stock.requests.Remove(this);
        }
    }
}
