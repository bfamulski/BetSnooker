import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

import { Match, RoundInfo, Bet, RoundBets } from '../_models';
import { SnookerFeedService, BetsService } from '../_services';

@Component({
  selector: 'app-bets',
  templateUrl: './bets.component.html',
  styleUrls: ['./bets.component.less']
})
export class BetsComponent implements OnInit {
  private bets: Bet[];
  private roundInfo: RoundInfo;

  loading = false;
  error = '';

  constructor(private betsService: BetsService,
              private snookerFeedService: SnookerFeedService) { }

  ngOnInit() {
    this.loading = true;

    const currentRoundInfoRequest = this.snookerFeedService.getCurrentRoundInfo();
    const betsRequest = this.betsService.getUserBets();

    forkJoin([currentRoundInfoRequest, betsRequest]).subscribe(results => {
      if (results[0] != null && results[1] != null) {
        this.roundInfo = results[0];
        this.bets = results[1].matchBets;
      } else {
        this.error = 'No bets available at this moment';
      }

      this.loading = false;
    }, error => {
      this.error = error;
      this.loading = false;
    });
  }

  submit() {
    const roundBets = new RoundBets({ roundId: this.roundInfo.round, distance: this.roundInfo.distance, matchBets: this.bets });
    this.betsService.submitBets(roundBets).subscribe();
  }
}
