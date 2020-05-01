using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class InfoBox : MonoBehaviour
{
    System.DateTime delay_start_time;

    [SerializeField]
    Text name_text = null, info_text = null;

    [SerializeField]
    RectTransform info_container = null;

    [SerializeField]
    int base_width = 0,
        base_height = 0;

    [SerializeField]
    float delay = 0;

    [SerializeField]
    CanvasGroup canvas_group = null;

    void Start()
    {
        delay_start_time = System.DateTime.Now;
        canvas_group.alpha = 0;
    }

    void Update()
    {
        HasInfos has_info = InputUtility.GetElementTouched<HasInfos>();
        if (has_info == null)
        {
            canvas_group.alpha = 0;
            return;
        }


        if (InputUtility.DidMouseMove)
            delay_start_time = System.DateTime.Now;

        float target_alpha = 0;
        if ((System.DateTime.Now - delay_start_time).TotalSeconds >= delay)
            target_alpha = 1;

        canvas_group.alpha = Mathf.Lerp(canvas_group.alpha, target_alpha, Time.deltaTime * 10);


        name_text.text = has_info.Name;

        List<string> lines = new List<string>();
        foreach (Info info in has_info.Infos)
            lines.Add(info.name + " : " + info.description);

        info_text.text = lines.Aggregate((a, b) => a + "\n" + b);
        info_text.fontSize = (int)(name_text.fontSize * 0.5f);

        float additional_width = lines.Max(line => line.Length) * 0.378f *
                                 info_text.fontSize * 0.5f;

        additional_width = Mathf.Max(additional_width,
                                     name_text.text.Length * 0.4f *
                                     name_text.fontSize * 0.5f);

        float additional_height = lines.Count() * 0.59f * 
                                  info_text.fontSize * 0.5f;

        (transform as RectTransform).sizeDelta =
            new Vector2(base_width + additional_width,
                        base_height + additional_height);

        info_container.sizeDelta = new Vector2(additional_width, additional_height);

        transform.position = Input.mousePosition;
    }

    public struct Info
    {
        public string name;
        public string description;

        public Info(string name_, string description_)
        {
            name = name_;
            description = description_;
        }
    }

    public interface HasInfos
    {
        string Name { get; }
        IEnumerable<InfoBox.Info> Infos { get; }
    }
}
