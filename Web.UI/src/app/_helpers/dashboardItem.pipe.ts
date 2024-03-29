import { Pipe, PipeTransform } from '@angular/core';
import { DashboardItem } from '../_models';

@Pipe({
    name: 'roundFilter',
    pure: false
})
export class DashboardItemRoundFilterPipe implements PipeTransform {
    transform(items: DashboardItem[], filter: any): any {
        if (!items || !filter) {
            return items;
        }

        // filter items array, items which match and return true will be
        // kept, false will be filtered out
        return items.filter(item => item.roundId === filter.round);
    }
}
