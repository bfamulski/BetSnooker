import { Component } from '@angular/core';
import { forkJoin } from 'rxjs';

import { Match, RoundInfo, RoundBets, DashboardItem, User } from '../_models';
import { SnookerFeedService, BetsService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent {

  private matches: Match[];
  private users: User[];
  private eventRounds: RoundInfo[];
  private roundBets: RoundBets[];

  private dashboardItems: DashboardItem[] = [];

  loading = false;
  error = '';

  constructor(private snookerFeedService: SnookerFeedService,
              private betsService: BetsService,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.loading = true;

    const eventRoundsRequest = this.snookerFeedService.getEventRounds();
    const eventMatchesRequest = this.snookerFeedService.getEventMatches();
    const usersRequest = this.authenticationService.getUsers();

    forkJoin([eventRoundsRequest, eventMatchesRequest, usersRequest]).subscribe(results => {
      this.eventRounds = results[0];
      this.matches = results[1];
      if (!this.eventRounds || !this.matches) {
        this.error = 'No rounds or matches available for this event';
        this.loading = false;
        return;
      }

      this.users = results[2];

      this.betsService.getAllBets().subscribe(bets => {
        this.roundBets = bets;

        this.matches.forEach(match => {
          const dashboardItem = new DashboardItem({
            roundId: match.round,
            matchId: match.matchId,
            player1Id: match.player1Id,
            player1Name: match.player1Name,
            score1: match.score1,
            player2Id: match.player2Id,
            player2Name: match.player2Name,
            score2: match.score2,
            winnerId: match.winnerId,
            winnerName: match.winnerName,
            roundName: match.roundName,
            roundBestOf: match.distance * 2 - 1,
            userBets: {}
          });

          if (this.users && this.roundBets) {
            this.users.forEach(user => {
              const userRoundBets = this.roundBets.filter(b => b.userId === user.username);
              userRoundBets.forEach(userRoundBet => {
                const bet = userRoundBet.matchBets.find(b => b.matchId === match.matchId);
                if (bet) {
                  dashboardItem.userBets[user.username] = { betScore1: bet.score1, betScore2: bet.score2, scoreValue: bet.scoreValue };
                }
              });
            });
          }

          this.dashboardItems.push(dashboardItem);
        });

        this.loading = false;
      }, error => {
        this.error = error;
        this.loading = false;
      });
    }, error => {
      this.error = error;
      this.loading = false;
    });
  }
}
