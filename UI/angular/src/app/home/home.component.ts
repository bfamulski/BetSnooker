import { Component } from '@angular/core';
import { forkJoin } from 'rxjs';

import { environment } from '../../environments/environment';
import { Match, RoundInfo, RoundBets, DashboardItem, User } from '../_models';
import { SnookerFeedService, BetsService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent {

  private roundId = environment.roundId;

  private matches: Match[];
  private roundInfo: RoundInfo;
  private roundBets: RoundBets[];
  private users: User[];

  private dashboardItems: DashboardItem[] = [];

  loading = false;

  constructor(private snookerFeedService: SnookerFeedService,
              private betsService: BetsService,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.loading = true;

    const roundInfoRequest = this.snookerFeedService.getRoundInfo(this.roundId);
    const roundMatchesRequest = this.snookerFeedService.getRoundMatches(this.roundId);
    const betsRequest = this.betsService.getAllBets(this.roundId);
    const usersRequest = this.authenticationService.getUsers();

    forkJoin([roundInfoRequest, roundMatchesRequest, betsRequest, usersRequest]).subscribe(results => {
      this.roundInfo = results[0];
      this.matches = results[1];
      this.roundBets = results[2];
      this.users = results[3];

      this.matches.forEach(match => {
        const dashboardItem = new DashboardItem({
          roundId: this.roundId,
          matchId: match.matchId,
          player1Id: match.player1Id,
          player1Name: match.player1Name,
          score1: match.score1,
          player2Id: match.player2Id,
          player2Name: match.player2Name,
          score2: match.score2,
          winnerId: match.winnerId,
          winnerName: match.winnerName,
          roundName: this.roundInfo.roundName,
          roundBestOf: this.roundInfo.distance * 2 - 1,
          userBets: {}
        });

        this.users.forEach(user => {
          const userRoundBets = this.roundBets.find(b => b.userId === user.username);
          if (userRoundBets) {
            const bet = userRoundBets.matchBets.find(b => b.matchId === match.matchId);
            if (bet) {
              dashboardItem.userBets[user.username] = { betScore1: bet.score1, betScore2: bet.score2 };
            }
          }
        });

        this.dashboardItems.push(dashboardItem);
      });

      console.log(this.dashboardItems); // TODO: to be removed
      this.loading = false;
    });
  }
}
