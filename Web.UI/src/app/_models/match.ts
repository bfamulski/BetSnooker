export class Match {
    matchId: number;
    eventId: number;
    round: number;
    roundName: string;
    distance: number;
    number: number;
    player1Id: number;
    player1Name: string;
    score1: number;
    walkover1: boolean;
    player2Id: number;
    player2Name: string;
    score2: number;
    walkover2: boolean;
    winnerId: number;
    winnerName: string;
    unfinished: boolean;
    onBreak: boolean;
    scheduledDate: Date;
    startDate: Date;
    endDate: Date;
    actualStartDate: Date;
}
