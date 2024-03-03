﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Models
{
    [System.Serializable]
    public class Resource
    {
        private string name;
        private int quantity;

        public string Name { get => name; set => name = value; }
        public int Quantity { get => quantity; set => quantity = value; }

        public Resource(string name, int quantity)
        {
            this.name = name;
            this.quantity = quantity;
        }
    }
}
