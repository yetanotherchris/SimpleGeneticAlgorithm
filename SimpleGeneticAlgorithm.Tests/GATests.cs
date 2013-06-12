using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SimpleGeneticAlgorithm.Tests
{
    [TestFixture]
    public class GATests
    {
        [Test]
        public void WorldToString()
        {
			// Arrange
            World world = new World(6, 4, 0, 0);
            world.InitializePopulation();

			// Act
			string debugString = world.ToString();
            Console.WriteLine(world);

			// Assert
			Assert.That(debugString, Is.Not.Empty);
			Assert.That(debugString, Contains.Substring("("));
			Assert.That(debugString, Contains.Substring(")"));
			Assert.That(debugString, Contains.Substring(","));
        }

		[Test]
		public void GenomeFromString()
		{
			// Arrange
			Genome genome = Genome.FromString("111 011");

			// Act
			List<bool> genes = genome.Genes.ToList();
			int total = genome.Total;

			// Assert
			Assert.That(total, Is.EqualTo(10));
			Assert.That(genes.Count, Is.EqualTo(6));
			Assert.That(genes[0], Is.True);
			Assert.That(genes[1], Is.True);
			Assert.That(genes[2], Is.True);
			Assert.That(genes[3], Is.False);
			Assert.That(genes[4], Is.True);
			Assert.That(genes[5], Is.True);
		}

		[Test]
		public void GenomeToString()
		{
			// Arrange
			Genome genome = new Genome(6);
			genome.SetGeneOn(0);
			genome.SetGeneOn(1);
			genome.SetGeneOn(2);
			genome.SetGeneOn(4);

			// Act
			string bitstring = genome.ToString();

			// Assert
			Assert.That(bitstring, Is.EqualTo("111 010 (7,2)"));
		}

		[Test]
		public void GenomeTotalShouldUseGenomeLengthOf6()
		{
			// Arrange
			Genome genome = Genome.FromString("111 111 111");
			Console.WriteLine(genome);

			// Act
			int total = genome.Total;

			// Assert
			Assert.That(total, Is.EqualTo(14));
		}

        [Test]
        public void GenomeTotal1()
        {
			// Arrange
			Genome genome = Genome.FromString("111 111");
            Console.WriteLine(genome);

			// Act
			int total = genome.Total;

			// Assert
			Assert.That(total, Is.EqualTo(14));
        }

        [Test]
        public void GenomeTotal2()
        {
			// Arrange
			Genome genome = Genome.FromString("000 000");
            Console.WriteLine(genome);

			// Act
			int total = genome.Total;

			// Assert
			Assert.That(total, Is.EqualTo(0));
        }

        [Test]
        public void GenomeTotal3()
        {
			// Arrange
			Genome genome = Genome.FromString("111 111");
            Console.WriteLine(genome);

			// Act
			int total = genome.Total;

			// Assert
			Assert.That(total, Is.EqualTo(14));
        }

		[Test]
		public void InitializePopulation()
		{
			// Arrange
			int worldSize = 4;
			int geneCount = 6;
			World world = new World(geneCount, worldSize, 0, 0);
			
			// Act
			world.InitializePopulation();

			// Assert
			Assert.That(world.Population.Count, Is.EqualTo(worldSize));
			Assert.That(world.Population[0].Genes.Count(), Is.EqualTo(geneCount));
		}

		[Test]
		public void BiasedRoulette()
		{
			//
			// Arrange
			//
			int worldSize = 4;
			int geneCount = 6;
			World world = new World(geneCount, worldSize, 0, 0);
			RandomStub random = new RandomStub(50);

			Genome genome1 = Genome.FromString("111 111"); // 14
			Genome genome2 = Genome.FromString("110 000"); // 6
			Genome genome3 = Genome.FromString("100 000"); // 4
			Genome genome4 = Genome.FromString("000 000"); // 0

			world.Population.Add(genome1);
			world.Population.Add(genome2);
			world.Population.Add(genome3);
			world.Population.Add(genome4);

			//
			// Act
			//
			
			// Total = 24
			// G1 = 12 / 24 * 100 = 50  <--- should be picked
			// G2 = 6 / 24 * 100 = 25
			// G3 = 4 / 24 * 100 = 16.6
			// G4 = 0

			Genome actualGenome1 = world.SpinBiasedRouletteWheel(random);

			// Assert
			Assert.That(world.Population.Count, Is.EqualTo(4));
			Assert.That(actualGenome1, Is.EqualTo(genome1));
		}

		[Test]
		public void CrossOver()
		{
			//
			// Arrange
			//
			int worldSize = 4;
			int geneCount = 6;
			int swapPosition = 3;
			int crossOverChance = 100;
			int mutationChance = 0;
			World world = new World(geneCount, worldSize, crossOverChance, mutationChance);
			RandomStub random = new RandomStub(swapPosition);

			// - Two genomes:
			// 000 111
			// 101 000
			//
			// - swapPosition = 4
			// 000 [111]
			// 101 [000]
			//
			//- New genomes:
			// 000 000
			// 101 111
			Genome genome1 = Genome.FromString("000 111");
			Genome genome2 = Genome.FromString("101 000");

			world.Population.Add(genome2);
			world.Population.Add(genome1);

			string g1 = world.Population[0].ToString();
			string g2 = world.Population[1].ToString();

			// Act
			Console.WriteLine("Before:");
			Console.WriteLine(g1);
			Console.WriteLine(g2);
			world.CrossOver(random);

			// Assert		
			string g1a = world.Population[0].ToString();
			string g2a = world.Population[1].ToString();

			Console.WriteLine("------");
			Console.WriteLine("After:");
			Console.WriteLine(g1a);
			Console.WriteLine(g2a);

			Assert.That(g1a, Is.StringStarting("000 000"));
			Assert.That(g2a, Is.StringStarting("101 111"));
		}

		[Test]
		public void Mutate()
		{
			//
			// Arrange
			//
			int worldSize = 4;
			int geneCount = 6;
			int swapPosition = 3;
			int crossOverChance = 0;
			int mutationChance = 100;
			RandomStub random = new RandomStub(0, 5);
			
			World world = new World(geneCount, worldSize, crossOverChance, mutationChance);

			// - Two genomes:
			// 000 111
			// 111 001
			//
			// Mutate both using position 1 and 5:
			//
			// 0[0]0 11[1] = 0[1]0 11[0]
			// 1[1]1 00[1] = 0[1]0 11[1]

			Genome genome1 = Genome.FromString("000 111");
			Genome genome2 = Genome.FromString("111 001");

			world.Population.Add(genome1);
			world.Population.Add(genome2);

			string g1 = genome1.ToString();
			string g2 = genome2.ToString();

			// Act
			Console.WriteLine("Before:");
			Console.WriteLine(g1);
			Console.WriteLine(g2);
			world.Mutate(random);

			// Assert		
			string g1a = genome1.ToString();
			string g2a = genome2.ToString();

			Console.WriteLine("------");
			Console.WriteLine("After:");
			Console.WriteLine(g1a);
			Console.WriteLine(g2a);

			Assert.That(g1a, Is.StringStarting("100 110"));
			Assert.That(g2a, Is.StringStarting("111 001"));
		}

		[Test]
		public void MutateShouldCheckMutationChance()
		{
			// Arrange
			int worldSize = 4;
			int geneCount = 6;
			int swapPosition = 3;
			int crossOverChance = 0;
			int mutationChance = 0;
			RandomStub random = new RandomStub(0, 5);
			
			World world = new World(geneCount, worldSize, crossOverChance, mutationChance);

			Genome genome1 = Genome.FromString("000 111");
			Genome genome2 = Genome.FromString("111 001");
			world.Population.Add(genome1);
			world.Population.Add(genome2);

			// Act
			world.Mutate(random);
			string g1 = world.Population[0].ToString();
			string g2 = world.Population[1].ToString();

			// Assert
			Assert.That(g1, Is.StringStarting("000 111"));
			Assert.That(g2, Is.StringStarting("111 001"));
		}

		[Test]
		public void NextGeneration()
		{
			// Arrange
			int worldSize = 4;
			int geneCount = 6;
			int crossOverChance = 30;
			int mutationChance = 5;
			World world = new World(geneCount, worldSize, crossOverChance, mutationChance);
			world.InitializePopulation();

			// Act
			int generation = 0;
			Genome champion = null;
			while (champion == null)
			{
				world.Mutate();
				world.CrossOver();
				world.NextGeneration();
				Console.WriteLine(world);

				generation++;
				champion = world.GetChampion();
			}

			Console.WriteLine("A new leader is born! generation {0} - {1}", generation, champion);
		}
    }
}
