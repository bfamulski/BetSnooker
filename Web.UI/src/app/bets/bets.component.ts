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
  roundBets: RoundBets[];
  currentRoundBets: RoundBets;
  nextRoundBets: RoundBets;

  currentRoundMatchBets: Bet[];
  nextRoundMatchBets: Bet[];
  nextRoundMatchBetsAvailable: boolean;

  currentRoundInfo: RoundInfo;
  nextRoundInfo: RoundInfo;

  loading = false;
  successfulSubmit = false;
  error = '';
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
    this.noBetsAvailable = false;

    const eventRoundsRequest = this.snookerFeedService.getEventRounds();
    const betsRequest = this.betsService.getUserBets();

    forkJoin([eventRoundsRequest, betsRequest]).subscribe(results => {
      if (results[0] != null && results[1] != null) {
        const eventRounds = results[0];

        this.roundBets = results[1];
        if (this.roundBets.length === 0) {
          this.noBetsAvailable = true;
          return;
        }

        this.currentRoundBets = this.roundBets[0];
        this.currentRoundInfo = eventRounds.find(r => r.round === this.currentRoundBets.roundId);

        if (this.roundBets.length > 1) {
          this.nextRoundBets = this.roundBets[1];
          this.nextRoundInfo = eventRounds.find(r => r.round === this.nextRoundBets.roundId);

          this.nextRoundMatchBets = this.nextRoundBets.matchBets.sort(this.compareBets);
          this.nextRoundMatchBetsAvailable = this.nextRoundMatchBets.filter(bet => bet.player1Name && bet.player2Name).length > 0;

          if (this.nextRoundMatchBetsAvailable) {
            this.nextRoundMatchBets.forEach(bet => bet.formattedStartDate = this.convertToLocalDateTime(bet.matchStartDate));
            this.currentRoundMatchBets = this.currentRoundBets.matchBets.filter(bet => bet.active).sort(this.compareBets);
          } else {
            this.currentRoundMatchBets = this.currentRoundBets.matchBets.sort(this.compareBets);
          }

          // TODO: this piece of code could be extracted to a separate method
          if (this.currentRoundBets.updatedAt && this.nextRoundBets.updatedAt) {
            let latestUpdatedAt: Date;
            if (this.currentRoundBets.updatedAt > this.nextRoundBets.updatedAt) {
              latestUpdatedAt = this.currentRoundBets.updatedAt;
            } else {
              latestUpdatedAt = this.nextRoundBets.updatedAt;
            }

            this.lastUpdatedAt = `${this.convertToLocalTime(new Date(latestUpdatedAt))}`;
          } else {
            if (this.currentRoundBets.updatedAt) {
              this.lastUpdatedAt = `${this.convertToLocalTime(new Date(this.currentRoundBets.updatedAt))}`;
            }
          }
          // end of TODO
        } else {
          this.currentRoundMatchBets = this.currentRoundBets.matchBets.sort(this.compareBets);

          if (this.currentRoundBets.updatedAt) {
            this.lastUpdatedAt = `${this.convertToLocalTime(new Date(this.currentRoundBets.updatedAt))}`;
          }
        }

        this.currentRoundMatchBets.forEach(bet => bet.formattedStartDate = this.convertToLocalDateTime(bet.matchStartDate));

        if (this.currentRoundMatchBets.length === 0 && !this.nextRoundMatchBetsAvailable) {
          this.noBetsAvailable = true;
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

    let roundBets: RoundBets[];

    const currentRoundBets = new RoundBets({
      roundId: this.currentRoundInfo.round,
      distance: this.currentRoundInfo.distance,
      matchBets: this.currentRoundMatchBets.filter(bet => bet.active)
    });

    if (this.nextRoundInfo) {
      const nextRoundBets = new RoundBets({
        roundId: this.nextRoundInfo.round,
        distance: this.nextRoundInfo.distance,
        matchBets: this.nextRoundMatchBets.filter(bet => bet.active)
      });

      roundBets = [currentRoundBets, nextRoundBets];
    } else {
      roundBets = [currentRoundBets];
    }

    this.betsService.submitBets(roundBets).subscribe(() => {
      this.successfulSubmit = true;
      this.lastUpdatedAt = `${this.convertToLocalTime(new Date(Date.now()))}`;
      this.betsChanged = false;
    }, error => {
      this.error = error;
    });
  }

  canSubmit(): boolean {
    this.validateBets(this.currentRoundInfo, this.currentRoundMatchBets);
    this.validateBets(this.nextRoundInfo, this.nextRoundMatchBets);
    return this.invalidBets.length === 0 && this.betsChanged;
  }

  private validateBets(roundInfo: RoundInfo, bets: Bet[]) {
    if (!roundInfo) {
      return;
    }

    const maxScore = roundInfo.distance;
    this.invalidBets = bets.filter(bet => (bet.score1 == null && bet.score2 != null)
                                          || (bet.score1 != null && bet.score2 == null)
                                          || (bet.score1 != null && bet.score2 != null
                                             && (bet.score1 === bet.score2 || bet.score1 > maxScore || bet.score2 > maxScore
                                             || bet.score1 < 0 || bet.score2 < 0 || (bet.score1 < maxScore && bet.score2 < maxScore))));
  }

  private convertToLocalTime(dateTime: Date) {
    if (dateTime) {
      return `${dateTime.toISOString().slice(0, 10)} ${dateTime.toLocaleTimeString('en-GB')}`;
    } else {
      return 'N/A';
    }
  }

  private convertToLocalDateTime(dateTime: Date) {
    if (dateTime) {
      const localDateTime = new Date(dateTime);
      const day = localDateTime.getDate().toString().padStart(2, '0');
      const month = (localDateTime.getMonth() + 1).toString().padStart(2, '0');
      const time = localDateTime.toLocaleTimeString('en-GB', {hour: 'numeric', minute: 'numeric'});
      return `${day}/${month} ${time}`;
    } else {
      return 'N/A';
    }
  }

  inputChanged() {
    this.betsChanged = true;
  }

  dismissSubmissionAlert() {
    this.successfulSubmit = false;
  }

  private compareBets(bet1: Bet, bet2: Bet) {
    if (bet1.matchStartDate < bet2.matchStartDate) {
      return -1;
    }
    if (bet1.matchStartDate > bet2.matchStartDate) {
      return 1;
    }
    return 0;
  }
}
