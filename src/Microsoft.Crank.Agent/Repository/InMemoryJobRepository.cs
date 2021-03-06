﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Crank.Models;

namespace Repository
{
    public class InMemoryJobRepository : IJobRepository
    {
        private readonly object _lock = new object();
        private readonly List<Job> _items = new List<Job>();
        private int _nextId = 1;

        public Job Add(Job item)
        {
            if (item.Id != 0)
            {
                throw new ArgumentException("item.Id must be 0.");
            }

            lock (_lock)
            {
                var id = _nextId;
                _nextId++;
                item.Id = id;
                _items.Add(item);
                return item;
            }
        }

        public Job Find(int id)
        {
            lock (_lock)
            {
                var items = _items.Where(job => job.Id == id);
                Debug.Assert(items.Count() == 0 || items.Count() == 1);
                return items.FirstOrDefault();
            }
        }

        public IEnumerable<Job> GetAll()
        {
            lock (_lock)
            {
                return _items.ToArray();
            }
        }

        public Job Remove(int id)
        {
            lock (_lock)
            {
                var item = Find(id);
                if (item == null)
                {
                    throw new ArgumentException($"Could not find item with Id '{id}'.");
                }
                else
                {
                    _items.Remove(item);
                    return item;
                }
            }
        }

        public void Update(Job item)
        {
            lock (_lock)
            {
                var oldItem = Find(item.Id);
                _items[_items.IndexOf(oldItem)] = item;
            }
        }
    }
}
