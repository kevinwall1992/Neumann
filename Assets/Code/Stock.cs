using UnityEngine;
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
        foreach (Request request in requests)
            foreach (Resource resource in request.UsagePerSecond.Resources)
                if (!Pile.Resources.Contains(resource))
                    Pile.PutIn(resource, 0);

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

    public Request MakeRequest(Resource resource, float usage_per_second)
    {
        Pile usage_per_second_pile = new Pile();
        usage_per_second_pile.PutIn(resource, usage_per_second);

        return MakeRequest(usage_per_second_pile);
    }

    public Request MakeRequest()
    {
        Request request = new Request(this, new Pile());
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

        internal Request(Stock stock_, Pile usage_per_second )
        {
            stock = stock_;

            UsagePerSecond = usage_per_second;

            Disbursement = new Pile();
        }

        public void Revoke()
        {
            stock.requests.Remove(this);
        }
    }
}
