using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleGeneticAlgorithm
{
    public class Genome
    {
        private bool[] _genes;
        private static Random _random = new Random();
	    public Guid Id { get; set; }

		public IEnumerable<bool> Genes
		{
			get { return _genes; }
		}

		public int Total
		{
			get
			{
				// For the dice example
				return TotalForDiceExample();
				
				// return GetTotal();
			}
		}

        public Genome(int maxGenes)
        {
            _genes = new bool[maxGenes];
			Id = Guid.NewGuid();
        }

		/// <summary>
		/// Sums up the total of all the genes by converting the bit string into a 
		/// 32 bit binary value. This method assumes the length of the genome is always 6.
		/// </summary>
		private int TotalForDiceExample()
		{
			string bitstring = "".PadLeft(32 - 3, '0');

			// First block
			string firstChunk = bitstring;
			firstChunk += _genes[0] ? "1" : "0";
			firstChunk += _genes[1] ? "1" : "0";
			firstChunk += _genes[2] ? "1" : "0";
			int total1 = Convert.ToInt32(firstChunk, 2);

			// Second block
			string secondChunk = bitstring;
			secondChunk += _genes[3] ? "1" : "0";
			secondChunk += _genes[4] ? "1" : "0";
			secondChunk += _genes[5] ? "1" : "0";
			int total2 = Convert.ToInt32(secondChunk, 2);

			return total1 + total2;
		}

		private int GetTotal()
		{
			string bitstring = "".PadLeft(32 - _genes.Length, '0');
			bitstring += string.Join("", _genes.Select(g => g == true ? "1": "0"));

			return Convert.ToInt32(bitstring);
		}

		/// <summary>
		/// Creates a <see cref="Genome"/> from a string of bits.
		/// </summary>
		/// <param name="bitString">A bit string, e.g. 100 001</param>
		/// <returns></returns>
		public static Genome FromString(string bitString)
		{
			if (string.IsNullOrEmpty(bitString))
				throw new ArgumentNullException("bitString", "bitString parameter is empty");

			bitString = bitString.Replace(" ", "");

			Genome genome = new Genome(bitString.Length);
			for (int i = 0; i < bitString.Length; i++)
			{
				if (bitString[i] != '0')
					genome.SetGeneOn(i);
			}

			return genome;
		}

        public void RandomizeGeneValues()
        {
            for (int i = 0; i < _genes.Length; i++)
			{
                // The gene is "1" when the rng is over 50
                if (_random.Next(1, 100) > 50)
                {
                    _genes[i] = true;
                }
                else
                {
                    _genes[i] = false;
                }
			}
        }

        public void SetGeneOn(int gene)
        {
            _genes[gene] = true;
        }

        public void SetGeneOff(int gene)
        {
            _genes[gene] = false;
        }

		public void SwapWith(Genome genome, int toPosition)
		{
			List<bool> sourceGenes = genome.Genes.ToList();
			for (int i = 0; i < toPosition; i++)
			{
				_genes[i] = sourceGenes[i];
			}
		}

		public void SwapGenes(int position1, int position2)
		{
			bool position1Value = _genes[position1];
			bool position2Value = _genes[position2];
			_genes[position1] = position2Value;
			_genes[position2] = position1Value;
		}

        public override string ToString()
        {
            string currentGene = "";
            string allGenes = "";
            string currentTotal = "";
            List<int> genomeTotal = new List<int>();

            // Splits the genome into blocks of 3 bits
            for (int i = 0; i < _genes.Length; i++)
            {
                if (i > 0 && i % 3 == 0)
                {
                    genomeTotal.Add(Convert.ToInt32(currentTotal, 2));
                    currentTotal = "";   
                }

                if (i > 0 && i % 3 == 0 && i < _genes.Length)
                {
                    allGenes += " ";
                }

                currentGene = _genes[i] ? "1" : "0";
                currentTotal += currentGene;

                if (i == _genes.Length - 1)
                {
                    genomeTotal.Add(Convert.ToInt32(currentTotal, 2));
                }

                allGenes += currentGene;
            }

            allGenes += string.Format(" ({0})", string.Join(",", genomeTotal));
            return allGenes;
        }

		public Genome Clone()
		{
			Genome clonedGenome = new Genome(_genes.Length);
			List<bool> genes = Genes.ToList();
			for (int i = 0; i < _genes.Length; i++)
			{
				clonedGenome._genes[i] = genes[i];
			}

			return clonedGenome;
		}

		public override bool Equals(object obj)
		{
			Genome genome = obj as Genome;
			if (genome == null)
				return false;

			return genome.Id == Id;
		}
	}
}
