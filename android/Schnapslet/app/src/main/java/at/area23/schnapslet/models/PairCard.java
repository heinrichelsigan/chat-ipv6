/*
 *
 * @author           Heinrich Elsigan root@darkstar.work
 * @version          V 1.6.9
 * @since            API 26 Android Oreo 8.1
 *
 */
/*
   Copyright (C) 2000 - 2023 Heinrich Elsigan root@darkstar.work

   Schnapslet java applet is free software; you can redistribute it and/or
   modify it under the terms of the GNU Library General Public License as
   published by the Free Software Foundation; either version 2 of the
   License, or (at your option) any later version.
   See the GNU Library General Public License for more details.

*/
package at.area23.schnapslet.models;

import at.area23.schnapslet.constenum.CARDCOLOR;

/**
 * PairCard class represents a pair (marriage) of cards.
 *
 * @see <a href="https://github.com/heinrichelsigan/schnapslet/wiki</a>
 */
public class PairCard extends  TwoCards {
    
	boolean atou = false;
	CARDCOLOR cardColor = CARDCOLOR.NONE; // Color of Pair
	char color = 'n';   // 4 colors and 'n' for unitialized
    int pairValue = 20;      // 5 values and 0 for unitialized

    /**
     * Constructor PairCard()
     */
    public PairCard() {
        super();
        this.color = 'n';
        this.pairValue = 20;
    }

	/**
	 * Constructor PairCard
	 * @param cardColor represents the color of the pair
	 * @param atouColor represents the color of Atou card
	 */
	public PairCard(CARDCOLOR cardColor, CARDCOLOR atouColor) {
		this();
		this.cardColor = cardColor;
		this.color = (char)cardColor.getChar();

		int queenNumValue = 1;
		if (cardColor == CARDCOLOR.SPADES) queenNumValue += 5;
		if (cardColor == CARDCOLOR.DIAMONDS) queenNumValue += 10;
		if (cardColor == CARDCOLOR.CLUBS) queenNumValue += 15;
		int kingNumValue = queenNumValue + 1;

		cards[0] = new Card(queenNumValue, atouColor.getChar());
		cards[1] = new Card(kingNumValue, atouColor.getChar());

		this.pairValue = 20;
		if (cardColor.getChar() == atouColor.getChar()) {
			this.atou = true;
			this.pairValue = 40;
		}
	}

	/**
	 * Constructor PairCard
	 * @param queenCard - the Queen in that pair
	 * @param kingCard - the King in that pair
	 */
	public PairCard(Card queenCard, Card kingCard) {
		super(queenCard, kingCard);
		if (cardSumValue != 7) {
			throw new RuntimeException("Sum of queen + king in pair must be 7!");
		}
		this.cardColor = queenCard.cardColor;
		this.color = (char)queenCard.cardColor.getChar();
		char kingColor = (char)kingCard.cardColor.getChar();
		if (kingColor != this.color) {
			throw new RuntimeException("Queen " + color + " & King " + kingColor + " must have same colors!");
		}
		cards[0] = new Card(queenCard);
		cards[1] = new Card(kingCard);
		if (queenCard.isAtou()) {
			this.atou = true;
			this.pairValue = 40;
		}
    }
}