export class DashboardItem {
  roundId: number;
  matchId: number;
  player1Id: number;
  player1Name: string;
  score1: string;
  player2Id: number;
  player2Name: string;
  score2: string;
  winnerId: number;
  winnerName: string;
  roundDistance: number;

  status: MatchStatus;

  userBets: { [userId: string]: { betScore1?: number, betScore2?: number, scoreValue?: number } };

  public constructor(fields?: Partial<DashboardItem>) {
    Object.assign(this, fields);
  }
}

export class MatchStatus {
  status: EMatchStatus;
  description: string;

  public constructor(fields?: Partial<MatchStatus>) {
    Object.assign(this, fields);
  }
}

export enum EMatchStatus {
  NotStarted = 1,
  Ongoing,
  Finished
}
