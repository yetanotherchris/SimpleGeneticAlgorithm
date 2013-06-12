using System;

namespace SimpleGeneticAlgorithm.Tests
{
	public class RandomStub : Random
	{
		private int[] _randomNumbers;
		private int _currentPosition;

		public RandomStub(params int[] randomNumbers)
		{
			_randomNumbers = randomNumbers;
		}

		public override int Next(int minValue, int maxValue)
		{
			if (++_currentPosition > _randomNumbers.Length -1)
				_currentPosition = 0;

			int result = _randomNumbers[_currentPosition];

			return result;
		}
	}
}