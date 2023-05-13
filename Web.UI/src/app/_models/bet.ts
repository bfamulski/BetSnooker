export class EventBets {
    userId: string;
    roundBets: RoundBets[];
    userScore: UserScore;
}

export class UserScore {
    matchesFinished: number;
    eventScore: number;
    correctWinners: number;
    exactScores: number;
    correctWinnersAccuracy: number;
    exactScoresAccuracy: number;
    averageError: number;
    isWinner: boolean;
}

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
    active: boolean;
    player1Id: number;
    player1Name: string;
    score1?: number;
    player2Id: number;
    player2Name: string;
    score2?: number;
    scoreValue: number;
    error: number;
    betPlaced: boolean;
    matchStartDate: Date;

    formattedStartDate: string;
}
