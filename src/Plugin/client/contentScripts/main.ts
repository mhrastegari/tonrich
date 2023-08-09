import tonrich from './tonrich';
import fragment from './fargment';
import tonscan from './tonscan';
import tonviwer from './tonviwer';

addTonrichBadge();

function addTonrichBadge() {
  let tonrich: tonrich | null = null;
  if (window.location.host.indexOf('fragment.com') !== -1) {
    tonrich = new fragment();
  } else if (window.location.host.indexOf('tonviewer.com') !== -1) {
    tonrich = new tonviwer();
  } else if (window.location.host.indexOf('tonscan.org') !== -1) {
    tonrich = new tonscan();
  }

  tonrich?.addBadge();
}
