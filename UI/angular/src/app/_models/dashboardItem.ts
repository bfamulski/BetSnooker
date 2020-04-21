export class DashboardItem {
  roundId: number;
  matchId: number;
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
  //scheduledDate: Date;
  //unfinished: boolean;
  roundName: string; // TODO: is it needed?
  roundBestOf: number; // TODO: is it needed?

  userBets: { [userId: string]: { betScore1?: number, betScore2?: number, scoreValue?: number } };

  public constructor(fields?: Partial<DashboardItem>) {
    Object.assign(this, fields);
  }
}
