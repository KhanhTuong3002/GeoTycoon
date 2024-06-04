using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string id { get; set; }
    public string title { get; set; }
    public int provinceId { get; set; }
    public object province { get; set; }
    public string userId { get; set; }
    public object user { get; set; }
    public object images { get; set; }
    public string content { get; set; }
    public string option1 { get; set; }
    public string option2 { get; set; }
    public string option3 { get; set; }
    public string option4 { get; set; }
    public string answer { get; set; }
    public string description { get; set; }
    public DateTime published { get; set; }
    public object lastUpdated { get; set; }
    public DateTime createdAt { get; set; }
}
