# Fun with Poker

## Overview

Poker is a game played with a standard 52-card deck of cards ([wikipedia deck](https://en.wikipedia.org/wiki/Standard_52-card_deck)), in which players attempt to make the best possible 5-card hand according to the ranking of the [categories of poker hands](http://www.pokerlistings.com/poker-hand-ranking). If you are unfamiliar with poker we recommend that you familiarize yourself with this list. The provided link also has a short video explaining how these hands work.

## In this challenge, you may assume:

* A single 52 card deck will be in use
* No wild cards
* Aces are treated as high cards only

Cards will be represented by their number or first letter for the non-numeric cards (J, Q, K, A) and the suits will be represented by their first letter (H, C, D, S) and stored as a JSON array. So for example a hand J♥ 4♣ 4♠ J♣ 9♥ will be represented as:

`["JH", "4C", "4S", "JC", "9H"]`

When a category involves less than 5 cards, the next highest cards are added as “kickers” for the sake of breaking ties.  For example, a pair of queens with a king beats a pair of queens with a 10.

## Tasks

1. Write a function that takes a 5-card hand as a JSON array and determines its category, with any tie breaking information that is necessary.  For example, the input  ["JH", "4C", "4S", "JC", "9H"] would have the value of two pair: jacks and 4s with a 9 kicker. You may choose your own representation for the output.

2. Write a function that takes 2 or more 5-card hands and determines the winner.

3. Some poker variations use more than 5 cards per player, and the player chooses the best subset of 5 cards to play.  Write a function that takes 5 or more cards and returns the best 5-card hand that can be made with those cards.  For example, the input [“3H”, “7S”, “3S”, “QD”, “AH”, “3D”, “4S”] should return [“3H”, “3S”, “3D”, “AH”, “QD”], which is a 3-of-a-kind with 3s, ace and queen kickers.
