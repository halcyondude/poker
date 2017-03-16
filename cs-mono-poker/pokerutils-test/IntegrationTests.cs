using System;
using System.Collections.Generic;
using NUnit.Framework;
using pokerutils;

namespace pokerutils_test
{
    [TestFixture]
    public class IntegrationTests
    {

        [Test]
        public void TestCompleteExamplesFromAssignment()
        {
            /*
                Some poker variations use more than 5 cards per player, and the player chooses the best subset of
                5 cards to play. Write a function that takes 5 or more cards and returns the best 5-card hand that
                can be made with those cards. For example, the input

                [\"3H\", \"7S\", \"3S\", \"QD\", \"AH\", \"3D\", \"4S\"]

                should return

                [\"3H\", \"3S\", \"3D\", \"AH\", \"QD\"]

                which is a 3-of-a-kind with 3s, ace and queen kickers.
            */
            
            // json is wonky with string literals...
            string sevenCardHand = "[\"3H\", \"7S\", \"3S\", \"QD\", \"AH\", \"3D\", \"4S\"]";
            string expectedOutput = "[\"3H\", \"3S\", \"3D\", \"AH\", \"QD\"]";

            Console.WriteLine("input : {0}", sevenCardHand);
            Console.WriteLine("output: {0}", expectedOutput);

            Hand inputHand = new Hand(sevenCardHand);
            List<Hand> winners = HandEvaluator.PickWinningHands(sevenCardHand);

        }

    }
}