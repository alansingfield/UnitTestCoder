using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;
using UnitTestCoder.Population.Maker;

namespace UnitTestCoder.Population.Tests.Maker
{
    [TestClass]
    public class ObjectPopulationMakerTests
    {
        private ObjectPopulationMaker _objectPopulationMaker;

        [TestInitialize]
        public void Init()
        {
            _objectPopulationMaker = new ObjectPopulationMaker(
                new ObjectDecomposer(new ValueLiteralMaker()),
                new ValueLiteralMaker(),
                new TypeNameLiteralMaker());
        }

        private string makeObjectPopulation(object arg)
        {
            return normalise(String.Join("\r\n", _objectPopulationMaker.Populate(arg, "model")));
        }

        private static Regex _normaliseRegex = new Regex(@"\s+");

        private string normalise(string arg)
        {
            return _normaliseRegex.Replace(arg, " ").Trim();
        }

        [TestMethod]
        public void ObjectPopulationSimple()
        {
            makeObjectPopulation(new Haribo() { Colour = "red" })
                .ShouldBe(normalise(@"
                    model = new Haribo();
                    model.Colour = ""red"";
                "));
        }

        [TestMethod]
        public void ObjectPopulationSimpleArray()
        {
            makeObjectPopulation(new[] {
                new Haribo() { Colour = "red" },
                new Haribo() { Colour = "green" },
                })
                .ShouldBe(normalise(@"
                    model = new Haribo[2];
                    model[0] = new Haribo();
                    model[0].Colour = ""red"";
                    model[1] = new Haribo();
                    model[1].Colour = ""green"";
                "));
        }

        [TestMethod]
        public void ObjectPopulationList()
        {
            makeObjectPopulation(new List<Haribo>() {
                new Haribo() { Colour = "red" },
                new Haribo() { Colour = "green" },
                })
                .ShouldBe(normalise(@"
                    var local0 = new Haribo[2];
                    local0[0] = new Haribo();
                    local0[0].Colour = ""red"";
                    local0[1] = new Haribo();
                    local0[1].Colour = ""green"";
                    model = new List<Haribo>(local0);
                "));
        }

        [TestMethod]
        public void ObjectPopulationNestedArray()
        {
            makeObjectPopulation(
                new[] {
                    new Sweetshop() { Name = "Wonka",
                    Sweets = new List<Haribo>() {
                        new Haribo() { Colour = "red" },
                        new Haribo() { Colour = "green" },
                        }},
                    new Sweetshop() { Name = "Fudge" },
                }
                )
                .ShouldBe(normalise(@"
                    model = new Sweetshop[2]; 
                    model[0] = new Sweetshop(); 
                    model[0].Name = ""Wonka""; 
                    var model_0__Sweets = new Haribo[2];
                    model_0__Sweets[0] = new Haribo();
                    model_0__Sweets[0].Colour = ""red"";
                    model_0__Sweets[1] = new Haribo();
                    model_0__Sweets[1].Colour = ""green"";
                    model[0].Sweets = new List<Haribo>(model_0__Sweets);
                    model[1] = new Sweetshop();
                    model[1].Name = ""Fudge"";
                    model[1].Sweets = null;
            "));
        }

        [TestMethod]
        public void ObjectPopulationDoubleNestedReference()
        {
            var sweets = new List<Haribo>() {
                        new Haribo() { Colour = "red" },
                        new Haribo() { Colour = "green" },
                        };

            makeObjectPopulation(
                new List<Town>()
                {
                    new Town() {
                        TownName = "Glasgow",
                        Sweetshops = new List<Sweetshop>() {
                            new Sweetshop()
                            {
                                Name = "Wonka",
                                Sweets = sweets
                            }
                        },
                    },
                    new Town() {
                        TownName = "Edinburgh",
                        Sweetshops = new List<Sweetshop>() {
                            new Sweetshop()
                            {
                                Name = "Cadbury",
                                Sweets = sweets
                            }
                        },
                    },

                })
                .ShouldBe(normalise(@"
                    var local0 = new Town[2];
                    local0[0] = new Town();
                    local0[0].TownName = ""Glasgow"";
                    var model_0__Sweetshops = new Sweetshop[1];
                    model_0__Sweetshops[0] = new Sweetshop();
                    model_0__Sweetshops[0].Name = ""Wonka"";
                    var model_0__Sweetshops_0__Sweets = new Haribo[2];
                    model_0__Sweetshops_0__Sweets[0] = new Haribo();
                    model_0__Sweetshops_0__Sweets[0].Colour = ""red"";
                    model_0__Sweetshops_0__Sweets[1] = new Haribo();
                    model_0__Sweetshops_0__Sweets[1].Colour = ""green"";
                    model_0__Sweetshops[0].Sweets = new List<Haribo>(model_0__Sweetshops_0__Sweets);
                    local0[0].Sweetshops = new List<Sweetshop>(model_0__Sweetshops);
                    local0[1] = new Town();
                    local0[1].TownName = ""Edinburgh"";
                    var model_1__Sweetshops = new Sweetshop[1];
                    model_1__Sweetshops[0] = new Sweetshop();
                    model_1__Sweetshops[0].Name = ""Cadbury"";
                    model_1__Sweetshops[0].Sweets = local0[0].Sweetshops[0].Sweets;
                    local0[1].Sweetshops = new List<Sweetshop>(model_1__Sweetshops);
                    model = new List<Town>(local0);
                "));
        }

        [TestMethod]
        public void ObjectPopulationNestedLists()
        {
            //var local0 = new Town[2];
            //local0[0] = new Town();
            //local0[0].TownName = "Glasgow";
            //var model_0__Sweetshops = new Sweetshop[1];
            //local0[0].Sweetshops[0] = new Sweetshop();
            //local0[0].Sweetshops[0].Name = "Wonka";
            //var model_0__Sweetshops_0__Sweets = new Haribo[2];
            //local0[0].Sweetshops[0].Sweets[0] = new Haribo();
            //local0[0].Sweetshops[0].Sweets[0].Colour = "red";
            //local0[0].Sweetshops[0].Sweets[1] = new Haribo();
            //local0[0].Sweetshops[0].Sweets[1].Colour = "green";
            //local0[0].Sweetshops[0].Sweets = new List<Haribo>(model_0__Sweetshops_0__Sweets);
            //local0[0].Sweetshops = new List<Sweetshop>(model_0__Sweetshops);
            //local0[1] = new Town();
            //local0[1].TownName = "Edinburgh";
            //var model_1__Sweetshops = new Sweetshop[1];
            //local0[1].Sweetshops[0] = new Sweetshop();
            //local0[1].Sweetshops[0].Name = "Cadbury";
            //local0[1].Sweetshops[0].Sweets = local0[0].Sweetshops[0].Sweets;
            //local0[1].Sweetshops = new List<Sweetshop>(model_1__Sweetshops);
            //var model = new List<Town>(local0);


            makeObjectPopulation(
                new List<Sweetshop>() {
                    new Sweetshop() { Name = "Wonka",
                    Sweets = new List<Haribo>() {
                        new Haribo() { Colour = "red" },
                        new Haribo() { Colour = "green" },
                        }},
                    new Sweetshop() { Name = "Fudge" },
                }
                )
                .ShouldBe(normalise(@"
                    var local0 = new Sweetshop[2];
                    local0[0] = new Sweetshop();
                    local0[0].Name = ""Wonka"";
                    var model_0__Sweets = new Haribo[2];
                    model_0__Sweets[0] = new Haribo();
                    model_0__Sweets[0].Colour = ""red"";
                    model_0__Sweets[1] = new Haribo();
                    model_0__Sweets[1].Colour = ""green"";
                    local0[0].Sweets = new List<Haribo>(model_0__Sweets);
                    local0[1] = new Sweetshop();
                    local0[1].Name = ""Fudge"";
                    local0[1].Sweets = null;
                    model = new List<Sweetshop>(local0);
            "));
        }



        [TestMethod]
        public void ObjectPopulationSelfReferentialArray()
        {
            var haribo = new Haribo() { Colour = "red" };

            makeObjectPopulation(new List<Haribo>() {
                haribo,
                haribo
                })
                .ShouldBe(normalise(@"
                    var local0 = new Haribo[2];
                    local0[0] = new Haribo();
                    local0[0].Colour = ""red"";
                    local0[1] = local0[0];
                    model = new List<Haribo>(local0);
                "));
        }

        [TestMethod]
        public void ObjectPopulationSelfReferentialArrayNested()
        {
            var haribo = new Haribo() { Colour = "red" };

            makeObjectPopulation(new List<Haribo>() {
                haribo,
                haribo
                })
                .ShouldBe(normalise(@"
                    var local0 = new Haribo[2];
                    local0[0] = new Haribo();
                    local0[0].Colour = ""red"";
                    local0[1] = local0[0];
                    model = new List<Haribo>(local0);
                "));
        }

        [TestMethod]
        public void ObjectPopulationKeepNonConstructableRoot()
        {
            // Bit of a cheat - we always assume root object is constructable
            var arg = new ConstructorHasParameters(1);

            makeObjectPopulation(arg).ShouldBe(normalise(@"
                model = new ConstructorHasParameters();
                model.Dummy = 1;
            "));
        }

        [TestMethod]
        public void ObjectPopulationOmitNonConstructable()
        {
            var arg = new RefersToConstructorWithParameters()
            {
                Foo = new ConstructorHasParameters(1)
            };

            makeObjectPopulation(arg).ShouldBe(normalise(@"
                model = new RefersToConstructorWithParameters();
                // model.Foo = new ConstructorHasParameters();
            "));
        }
    }

    public class Haribo
    {
        public string Colour { get; set; }
    }
    public class Sweetshop
    {
        public string Name { get; set; }
        public List<Haribo> Sweets { get; set; }
    }

    public class Town
    {
        public string TownName { get; set; }
        public List<Sweetshop> Sweetshops { get; set; }
    }

    public class ConstructorHasParameters
    {
        public int Dummy { get; set; }

        public ConstructorHasParameters(int dummy)
        {
            this.Dummy = dummy;
        }
    }

    public class RefersToConstructorWithParameters
    {
        public ConstructorHasParameters Foo { get; set; }
    }
}
