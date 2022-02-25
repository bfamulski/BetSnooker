export class RoundInfo {
    round: number;
    roundName: string;
    eventId: number;
    distance: number;
    numMatches: number;
    actualStartDate: Date;
    started: boolean;
    finished: boolean;
    isFinalRound: boolean;

    startDate: string; // this is only to format actualStartDate
}
