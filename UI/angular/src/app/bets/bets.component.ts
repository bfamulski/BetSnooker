import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

import { RoundInfo, Bet, RoundBets } from '../_models';
import { SnookerFeedService, BetsService } from '../_services';

@Component({
  selector: 'app-bets',
  templateUrl: './bets.component.html',
  styleUrls: ['./bets.component.less']
})
export class BetsComponent implements OnInit {
  private roundBets: RoundBets;
  private bets: Bet[];
  private roundInfo: RoundInfo;

  loading = false;
  successfulSubmit = false;
  error = '';
  validationErrors: string[] = [];
  noBetsAvailable = false;

  invalidBets: Bet[] = [];
  lastUpdatedAt = '';
  betsChanged = false;

  constructor(private betsService: BetsService,
              private snookerFeedService: SnookerFeedService) { }

  ngOnInit() {
    this.loading = true;
    this.successfulSubmit = false;
    this.error = '';
    this.validationErrors = [];
    this.noBetsAvailable = false;

    const currentRoundInfoRequest = this.snookerFeedService.getCurrentRoundInfo();
    const betsRequest = this.betsService.getUserBets();

    forkJoin([currentRoundInfoRequest, betsRequest]).subscribe(results => {
      if (results[0] != null && results[1] != null) {
        this.roundInfo = results[0];
        this.roundBets = results[1];
        this.bets = this.roundBets.matchBets;

        if (this.roundBets.updatedAt) {
          this.lastUpdatedAt = `${this.convertToLocalTime(new Date(this.roundBets.updatedAt))}`;
        }
      } else {
        this.noBetsAvailable = true;
      }

      this.loading = false;
    }, error => {
      this.error = error;
      this.loading = false;
    });
  }

  submit() {
    this.successfulSubmit = false;

    this.validationErrors = [];
    this.validateBets();
    if (this.invalidBets.length > 0) {
      this.invalidBets.forEach(bet => {
        this.validationErrors.push(`Bets validation error in: ${bet.player1Name} vs ${bet.player2Name}`);
      });

      return;
    }

    const roundBets = new RoundBets({ roundId: this.roundInfo.round, distance: this.roundInfo.distance, matchBets: this.bets });
    this.betsService.submitBets(roundBets).subscribe(result => {
      this.successfulSubmit = true;
      this.lastUpdatedAt = `${this.convertToLocalTime(new Date())}`;
      this.betsChanged = false;
    }, error => {
      this.error = error;
    });
  }

  validateBets() {
    const maxScore = this.roundInfo.distance;
    this.invalidBets = this.bets.filter(bet => (bet.score1 == null && bet.score2 != null)
                                            || (bet.score1 != null && bet.score2 == null)
                                            || (bet.score1 != null && bet.score2 != null
                                               && (bet.score1 === bet.score2 || bet.score1 > maxScore || bet.score2 > maxScore
                                               || bet.score1 < 0 || bet.score2 < 0 || (bet.score1 < maxScore && bet.score2 < maxScore))));
  }

  canSubmit(): boolean {
    this.validateBets();
    return this.invalidBets.length === 0 && this.betsChanged;
  }

  convertToLocalTime(dateTime: Date) {
    return `${dateTime.toISOString().slice(0, 10)} ${dateTime.toLocaleTimeString()}`;
  }

  inputChanged() {
    this.betsChanged = true;
  }
}
