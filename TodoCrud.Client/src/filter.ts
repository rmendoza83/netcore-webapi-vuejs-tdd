import numeral from 'numeral';
import moment from 'moment';

// Example of use: {{ $filters.currencyUSD(accountBalance) }}

const filter = {
  'formatInteger': (value: any) => {
    if (value) {
      return numeral(value).format('#,##0');
    }
  },
  'formatDecimal': (value: any) => {
    if (value) {
      return numeral(value).format('#,##0.00[00]');
    }
  },
  'formatCurrency': (value: any) => {
    if (value) {
      return numeral(value).format('$#,##0.00');
    }
  },
  'formatDate': (value: any) => {
    if (value) {
      return moment(value).format('DD/MM/YYYY');
    }
  },
  'formatTime': (value: any) => {
    if (value) {
      return moment(value).format('HH:mm:ss A');
    }
  },
  'formatDateTime': (value: any) => {
    if (value) {
      return moment(value).format('DD/MM/YYYY HH:mm:ss A');
    }
  }
}

export default filter;
