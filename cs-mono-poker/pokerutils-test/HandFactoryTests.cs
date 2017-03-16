using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using pokerutils;

namespace pokerutils_test
{
    [TestFixture]
    public class HandFactoryTests
    {
        [Test]
        public void TestSubset()
        {
            HandFactory hf = new HandFactory();
            hf.SubsetInternalTest();
        }

        [Test]
        public void TestGenHands()
        {
            List<Hand> allHands;

            // -----------------------------

            string[] seven = {"10C", "9C", "8C", "7C", "6C", "5C", "4C"};
            Hand sevenCardHand = new Hand(seven);

            allHands = HandFactory.CreateAllPossibleHands(sevenCardHand, 5);
            foreach(Hand h in allHands)
                Console.WriteLine(h);

            Assert.AreEqual(21, allHands.Count);

            // -----------------------------

            string[] eight = {"10C", "9C", "8C", "7C", "6C", "5C", "4C", "3C"};
            Hand eightCardHand = new Hand(eight);

            allHands = HandFactory.CreateAllPossibleHands(eightCardHand, 5);
            foreach(Hand h in allHands)
                Console.WriteLine(h);

            Assert.AreEqual(56, allHands.Count);

        }

    }
}