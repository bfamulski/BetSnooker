export class RoundBets {
    id: number;
    userId: string;
    updatedAt: Date;
    eventId: number;
    roundId: number;
    distance: number;
    matchBets: Bet[];

    public constructor(fields?: Partial<RoundBets>) {
      Object.assign(this, fields);
    }
}

export class Bet {
    id: number;
    matchId: number;
    player1Id: number;
    player1Name: string;
    score1?: number;
    player2Id: number;
    player2Name: string;
    score2?: number;
    scoreValue: number;
    error: number;
}
