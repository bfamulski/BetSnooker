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
  status: string;

  userBets: { [userId: string]: { betScore1?: number, betScore2?: number, scoreValue?: number } };

  public constructor(fields?: Partial<DashboardItem>) {
    Object.assign(this, fields);
  }
}
