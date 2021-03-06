﻿using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace CalcEngineTest
{
    public class TestData
    {
        private static List<Student> _students;

        public static List<Student> GetStudents()
        {
            if (_students == null)
            {
                _students = new List<Student>
                {
                    new Student
                    {
                        Id = 1,
                        Name = "zhangsan",
                        Age = 20,
                        Address = new Address
                        {
                            Province = "beijing",
                            City = "beijing"
                        },
                        Score = 80
                    },
                    new Student
                    {
                        Id = 2,
                        Name = "lisi",
                        Age = 30,
                        Address = new Address
                        {
                            Province = "shanghai",
                            City = "shanghai"
                        },
                        Score = 90
                    },
                    new Student
                    {
                        Id = 3,
                        Name = "wangwu",
                        Age = 40,
                        Address = new Address
                        {
                            Province = "guangzhou",
                            City = "guangzhou"
                        },
                        Score = 100
                    }
                };
            }

            return _students;
        }

        public static Student GetStudent()
        {
            return GetStudents()[0];
        }

        public static Dictionary<string, object> GetDictory()
        {
            return new Dictionary<string, object>()
            {
                {"key1","value1" },
                {"key2","value2" }
            };
        }
    }

    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Address Address { get; set; }

        public decimal Score { get; set; }
    }

    public class Address
    {
        public string Province { get; set; }

        public string City { get; set; }
    }
}
