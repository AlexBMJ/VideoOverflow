﻿namespace VideoOverflow.Infrastructure.Entities;
public class Comment
{
    //Attributes
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public int AttachedToResource { get; set; }
    public string Content { get; set; }
}