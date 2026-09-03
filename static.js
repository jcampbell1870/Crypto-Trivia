import { rewardConfig } from "./js/config.js";

const rewardPerPoint = 0.01;
const categories = [
  ["Geography", [
    ["Which continent contains the Andes Mountains?", ["Asia", "South America", "Europe", "Africa"], 1],
    ["What is the capital city of Canada?", ["Toronto", "Vancouver", "Ottawa", "Montreal"], 2],
    ["Which ocean lies between Africa and Australia?", ["Atlantic", "Indian", "Arctic", "Southern"], 1],
    ["The River Danube empties into which sea?", ["Black Sea", "Baltic Sea", "Red Sea", "North Sea"], 0],
    ["What is the largest island in the world?", ["Madagascar", "Borneo", "Greenland", "New Guinea"], 2]
  ]],
  ["Entertainment", [
    ["Which instrument typically has 88 keys?", ["Violin", "Piano", "Trumpet", "Flute"], 1],
    ["In filmmaking, what does a director use to signal the start of a take?", ["A clapboard", "A spotlight", "A script", "A microphone"], 0],
    ["What word describes a book that tells the story of a person's life written by someone else?", ["Memoir", "Autobiography", "Biography", "Novel"], 2],
    ["Which art form combines movement with music to tell a story?", ["Sculpture", "Ballet", "Photography", "Architecture"], 1],
    ["What is the term for the main character in a story?", ["Antagonist", "Narrator", "Protagonist", "Editor"], 2]
  ]],
  ["History", [
    ["Which ancient civilization built the pyramids at Giza?", ["Romans", "Egyptians", "Maya", "Vikings"], 1],
    ["The printing press in Europe is closely associated with whom?", ["Galileo", "Gutenberg", "Newton", "Da Vinci"], 1],
    ["Which document begins with the words 'We the People'?", ["Magna Carta", "U.S. Constitution", "Declaration of Independence", "Bill of Rights"], 1],
    ["What was the name of the trade route linking East Asia and Europe?", ["Amber Road", "Silk Road", "Spice Coast", "Royal Way"], 1],
    ["Which city was buried after the eruption of Mount Vesuvius in AD 79?", ["Athens", "Pompeii", "Carthage", "Sparta"], 1]
  ]],
  ["Arts & Literature", [
    ["What primary colors are mixed to make green paint?", ["Red and blue", "Blue and yellow", "Red and yellow", "Black and white"], 1],
    ["Who wrote the play Romeo and Juliet?", ["Charles Dickens", "William Shakespeare", "Jane Austen", "Mark Twain"], 1],
    ["What is a three-line Japanese poem traditionally called?", ["Sonnet", "Limerick", "Haiku", "Ode"], 2],
    ["Which material is commonly used for a watercolor painting?", ["Canvas", "Watercolor paper", "Marble", "Wood"], 1],
    ["What punctuation mark ends an exclamatory sentence?", ["Period", "Question mark", "Comma", "Exclamation mark"], 3]
  ]],
  ["Science & Nature", [
    ["What gas do plants absorb during photosynthesis?", ["Oxygen", "Nitrogen", "Carbon dioxide", "Helium"], 2],
    ["Which planet is known for its prominent rings?", ["Mars", "Saturn", "Venus", "Mercury"], 1],
    ["What is the process by which liquid water becomes water vapor?", ["Condensation", "Evaporation", "Freezing", "Melting"], 1],
    ["Which organ pumps blood through the human body?", ["Lung", "Liver", "Heart", "Kidney"], 2],
    ["What type of animal is a frog?", ["Reptile", "Amphibian", "Mammal", "Bird"], 1]
  ]],
  ["Sports & Leisure", [
    ["How many players from one team are on a basketball court at once?", ["Five", "Six", "Seven", "Eleven"], 0],
    ["What piece begins a chess game beside the king?", ["Rook", "Knight", "Queen", "Pawn"], 2],
    ["In tennis, what score follows 40–40?", ["Love", "Deuce", "Advantage", "Match point"], 1],
    ["Which sport uses a shuttlecock?", ["Squash", "Badminton", "Table tennis", "Lacrosse"], 1],
    ["What is the term for a score of zero in golf on a hole?", ["Birdie", "Bogey", "Par", "Eagle"], 2]
  ]]
];

let account = "", score = 0, answered = 0, active;
const $ = (id) => document.getElementById(id);
const formatReward = () => (score * rewardPerPoint).toFixed(4);

function drawBoard() {
  const board = $("board"); board.replaceChildren();
  categories.forEach(([name, clues], categoryIndex) => {
    const title = document.createElement("div"); title.className = "category"; title.textContent = name; board.append(title);
    clues.forEach((clue, clueIndex) => {
      const button = document.createElement("button"); button.className = "clue"; button.textContent = `$${(clueIndex + 1) * 100}`;
      button.disabled = clue.used; button.classList.toggle("used", clue.used);
      button.onclick = () => showQuestion(categoryIndex, clueIndex); board.append(button);
    });
  });
}
function showQuestion(categoryIndex, clueIndex) {
  active = { categoryIndex, clueIndex }; const [name, clues] = categories[categoryIndex], [question, options] = clues[clueIndex];
  $("question-category").textContent = name; $("question-text").textContent = question; $("answers").replaceChildren();
  options.forEach((option, index) => { const button = document.createElement("button"); button.textContent = option; button.onclick = () => answer(index); $("answers").append(button); });
  $("question").hidden = false;
}
function answer(index) {
  const clue = categories[active.categoryIndex][1][active.clueIndex]; clue.used = true; answered++;
  if (index === clue[2]) score += (active.clueIndex + 1) * 100;
  $("score").textContent = score; $("reward").textContent = formatReward(); $("question").hidden = true; drawBoard();
  if (answered === 30) completeGame();
}
async function completeGame() {
  $("game").hidden = true; $("final-score").textContent = score; $("final-reward").textContent = `${formatReward()} ${rewardConfig.tokenSymbol}`; $("complete").hidden = false;

  if (!account || !rewardConfig.tokenAddress || !rewardConfig.rewardVaultAddress) return;

  try {
    const sender = rewardConfig.rewardVaultAddress;
    const tokenAmount = BigInt(Math.round(Number(formatReward()) * 1e18));
    const toAddress = account.replace(/^0x/i, '').padStart(64, '0');
    const amountHex = tokenAmount.toString(16).padStart(64, '0');
    const data = `0xa9059cbb${toAddress}${amountHex}`;
    const txHash = await window.ethereum.request({
      method: 'eth_sendTransaction',
      params: [{ from: sender, to: rewardConfig.tokenAddress, data, value: '0x0' }]
    });
    console.log('Reward transfer submitted from treasury:', txHash);
  } catch (error) {
    console.warn('Reward transfer could not be submitted:', error.message || error);
  }
}
$("connect-wallet").onclick = async () => {
  if (!window.ethereum?.isMetaMask) return $("wallet-status").textContent = "MetaMask is required to connect a wallet.";
  try { [account] = await window.ethereum.request({ method: "eth_requestAccounts" }); $("wallet-status").textContent = `Connected: ${account.slice(0, 6)}…${account.slice(-4)}`; $("start-game").disabled = false; }
  catch { $("wallet-status").textContent = "Wallet connection was not approved."; }
};
$("start-game").onclick = () => { $("welcome").hidden = true; $("game").hidden = false; drawBoard(); };
$("play-again").onclick = () => { score = answered = 0; categories.forEach(([, clues]) => clues.forEach(clue => delete clue.used)); $("score").textContent = 0; $("reward").textContent = "0.0000"; $("complete").hidden = true; $("welcome").hidden = false; };
