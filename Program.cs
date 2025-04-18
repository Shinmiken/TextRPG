using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using System.IO;
using System.Text.Json;

namespace RPG
{
    //플레이어 클래스
    class Player
    {
        public string Name { get; set; } // 이름
        public string Job { get; set; } // 직업
        public int Level { get; set; } = 01; // 레벨
        public int Health { get; set; } = 100; // 체력
        public float Attack { get; set; } = 10; // 공격력
        public int Defense { get; set; } = 5; // 방어력
        public int Glod { get; set; } = 1500; // 골드
        public void MakeNameJob(string name, string job) // 이름, 직업 설정
        {
            Name = name;
            Job = job;
        }
    }

    //아이템 클래스
    class Item
    {
        public List<string> item { get; set; } = new List<string>(); // 내가 가지고 있는 아이템

        public List<string> itemStatus { get; set; } = new List<string>(); // 아이템 능력치

        public List<bool> isGet { get; set; } = new List<bool>(); // 아이템 착용 여부


        //아이템 착용 여부 변경
        public void change(int num)
        {
            while (item.Count <= num - 1)
            {
                isGet.Add(false);
            }
            isGet[num - 1] = !isGet[num - 1];
        }

        //아이템 착용
        public void WearItem(int num, Player player)
        {
            int x = num - 1;
            if (isGet[x]) // 착용 했으면 착용 표시 생성 및 능력치 추가
            {
                item[x] = item[x].Replace("- ", "- [E] ");
                if (item[x].Contains("공격력"))
                {
                    player.Attack += int.Parse(itemStatus[x]);
                }
                else if (item[x].Contains("방어력"))
                {
                    player.Defense += int.Parse(itemStatus[x]);
                }
            }
            else if (!isGet[x]) // 착용 해제시 착용 표시 삭제 및 능력치 추가
            {
                if(item[x].Contains("[E]"))
                {
                    item[x] = item[x].Replace("- [E] ", "- ");
                }
                if (item[x].Contains("공격력"))
                {
                    player.Attack -= int.Parse(itemStatus[x]);
                }
                else if (item[x].Contains("방어력"))
                {
                    player.Defense -= int.Parse(itemStatus[x]);
                }
            }
        }

        // 능력치 증가량 표시
        public int add()
        {
            int result = 0;
            for (int i = 0; i < item.Count; i++)
            {
                if (isGet[i] && item[i].Contains("공격력"))
                {
                    result += int.Parse(itemStatus[i]);
                }
            }
            return result;
        }

        // 방어력 증가량 표시
        public int def()
        {
            int result = 0;
            for(int i = 0; i < item.Count; i++)
            {
                if (isGet[i] && item[i].Contains("방어력"))
                {
                    result += int.Parse(itemStatus[i]);

                }
            }
            return result;
        }
    }

    //상점 클래스
    class Store : Item
    {
        static int itemNumber { get; set; } = 1; // 리스트에 차례대로 넣기 위한 변수

        public string[] store_item = // 상점 아이템 목록
        {
            "- 수련자 갑옷\t| 방어력 +5  | 수련에 도움을 주는 갑옷입니다.\t|  1000 G",
            "- 무쇠갑옷\t| 방어력 +9  | 무쇠로 만들어져 튼튼한 갑옷입니다.\t|  2000 G",
            "- 스파르타의 갑옷\t| 방어력 +15 | 스파르타의 전사들이 사용했다는 전설의 갑옷입니다.|  3500 G",
            "- 낡은 검\t| 공격력 +2  | 쉽게 볼 수 있는 낡은 검 입니다.\t|  600 G",
            "- 청동 도끼\t| 공격력 +5  |  어디선가 사용됐던거 같은 도끼입니다.\t|  1500 G",
            "- 스파르타의 창\t| 공격력 +7  | 스파르타의 전사들이 사용했다는 전설의 창입니다.\t|  1800 G",
            "- 스파르타의 방패\t| 방어력 +7  | 스파르타의 전사들이 사용했다는 전설의 방패입니다.\t|  1800 G",
            "- 엑스칼리버\t| 공격력 +20 | 전설 그자체. \t|  10000 G",
        };

        public string[] store_item_original = // 상점 아이템 목록 원본 / 가격 재생성을 위해 존재
        {
            "- 수련자 갑옷\t| 방어력 +5  | 수련에 도움을 주는 갑옷입니다.\t|  1000 G",
            "- 무쇠갑옷\t| 방어력 +9  | 무쇠로 만들어져 튼튼한 갑옷입니다.\t|  2000 G",
            "- 스파르타의 갑옷\t| 방어력 +15 | 스파르타의 전사들이 사용했다는 전설의 갑옷입니다.|  3500 G",
            "- 낡은 검\t| 공격력 +2  | 쉽게 볼 수 있는 낡은 검 입니다.\t|  600 G",
            "- 청동 도끼\t| 공격력 +5  |  어디선가 사용됐던거 같은 도끼입니다.\t|  1500 G",
            "- 스파르타의 창\t| 공격력 +7  | 스파르타의 전사들이 사용했다는 전설의 창입니다.\t|  1800 G",
            "- 스파르타의 방패\t| 방어력 +7  | 스파르타의 전사들이 사용했다는 전설의 방패입니다.\t|  1800 G",
            "- 엑스칼리버\t| 공격력 +20 | 전설 그자체. \t|  10000 G",
        };

        public List<string> store_Sell_item = new List<string>(); // 판매 아이템 목록

        public string[] itemPirce = { "1000", "2000", "3500", "600", "1500", "1800", "1800", "10000" }; // 아이템 가격

        // 아이템 구매
        public void BuyItem(int num, Item playerItem, Player player)
        {
            //구매 완료된 아이템은 구매 불가
            if (store_item[num - 1].Contains("구매완료"))
            {
                
                Console.WriteLine("구매할 수 없는 아이템입니다.");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                // 리스트 공간 확보
                while (playerItem.item.Count <= itemNumber - 1)
                {
                    playerItem.item.Add("");
                    playerItem.itemStatus.Add("");
                    store_Sell_item.Add("");
                    playerItem.isGet.Add(false);
                }
                // 구매한 아이템 구매완료 표시 생성 및 아이템 능력치 추출
                store_Sell_item[itemNumber - 1] = store_item[num - 1];
                string original = store_item[num - 1];
                int startIndex = original.IndexOf("-");
                int endIndex = original.IndexOf(".");
                string name = original.Substring(startIndex, endIndex - startIndex);

                playerItem.item[itemNumber - 1] = name;
                store_item[num - 1] = name + "\t|   구매완료";
                int str = name.IndexOf("+") + 1;
                playerItem.itemStatus[itemNumber - 1] = name.Substring(str, 2);
                player.Glod -= int.Parse(itemPirce[num - 1]); // 골드 차감
                itemNumber++;
            }
        }

        // 아이템 판매
        public void Sell_item(int num, Item playerItem)
        {
            // 판매 아이템과 원본 아이템 비교해서 아이템 위치 복구
            int var = store_Sell_item[num - 1].IndexOf(" +") + 1;
            string str = store_Sell_item[num - 1].Substring(var, 3);

            for (int i = 0; i < store_item.Length; i++)
            {
                if (store_item_original[i].Contains(str))
                {
                    store_item[i] = store_item_original[i];
                }
            }

            // 1. 판매 리스트에서 제거
            if (num - 1 < store_Sell_item.Count)
                store_Sell_item.RemoveAt(num - 1);

            // 2. 플레이어 인벤토리에서도 제거
            if (num - 1 < playerItem.item.Count)
                playerItem.item.RemoveAt(num - 1);

            //3. 아이템 능력치 제거
            if (num - 1 < playerItem.itemStatus.Count)
                playerItem.itemStatus.RemoveAt(num - 1);

            // 4. 아이템 착용 여부 제거
            if (num - 1 < playerItem.isGet.Count)
                playerItem.isGet.RemoveAt(num - 1);
            itemNumber--;
        }
    }

    //게임 저장
    class SaveData
    {
        public Player Player { get; set; }
        public Item Item { get; set; }
        public List<string> StoreItems { get; set; } // 배열 데이터만 따로 가져오기 위해 리스트로 가져오기
    }

    internal class Program
    {
        public static string job;
        static string name;
        static int dungeonNum = 0;
        static int isGameSave = 0;
        static int[] dungeonDef = { 5, 11, 17 };
        static int[] dungeonGlod = { 1000, 1700, 2500 };
        static string[] dungeonName = { "쉬운 ", "일반 ", "어려운 " };
        static Player player = new Player();
        static Item item = new Item();
        static Store store = new Store();
        static void Main(string[] args)
        {
            ShowStartView();
            if (isGameSave != 1)
            {
                GameName();
            }
            GameStart();
        }

        // 이름, 직업 선택 화면
        static void GameName()
        {
            string choice;
            int cnt = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("스파르타 던전에 오신것을 환영합니다.");
                if (cnt == 0)
                {
                    Console.WriteLine("이름을 정해주세요!\n");

                    name = Console.ReadLine();

                    Console.WriteLine($"입력하신 이름 {name}입니다.\n");

                    Console.WriteLine("1. 확인");
                    Console.WriteLine("2. 취소\n");

                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    Console.Write(">>\t");
                    choice = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("원하는 직업을 선택해주세요!\n");

                    Console.WriteLine("1. 전사");
                    Console.WriteLine("2. 도적\n");

                    job = Console.ReadLine();
                    if(job == "1")
                    {
                        job = "전사";
                    }
                    else if (job == "2")
                    {
                        job = "도적";
                    }
                    else
                    {
                        Console.Clear();
                        continue;
                    }

                    Console.WriteLine($"선택하신 직업은 {job} 입니다.\n");

                    Console.WriteLine("1. 확인");
                    Console.WriteLine("2. 취소\n");

                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    Console.Write(">>\t");
                    choice = Console.ReadLine();
                }

                if (choice == "1")
                {
                    Console.Clear();
                    if (cnt != 0)
                    {
                        break;
                    }
                    cnt++;
                }
                else if (choice == "2")
                {
                    Console.Clear();
                    continue;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.WriteLine("아무키나 누르세요.");
                    Console.ReadKey();
                }
            }
            player.MakeNameJob(name, job);
        }

        //게임 메인 화면
        static void GameStart()
        {
            Console.ForegroundColor = ConsoleColor.White;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("스파르타 마을에 오신것을 환영합니다.\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
                Console.WriteLine("1. 상태보기\n2. 인벤토리\n3. 상점\n4. 휴식하기\n5. 던전입장\n6. 게임 저장\n0. 게임종료\n");

                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>\t");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    ShowStatus();
                    break;
                }
                else if (choice == "2")
                {
                    ShowInventory();
                    break;
                }
                else if (choice == "3")
                {
                    ShowShop();
                    break;
                }
                else if (choice == "4")
                {
                    GetRest();
                    break;
                }
                else if (choice == "5")
                {
                    ShowDungeon();
                    break;
                }
                else if (choice == "6")
                {
                    SaveGame();
                    Console.WriteLine("아무키나 누르세요.");
                    Console.ReadKey();
                }
                else if (choice == "0")
                {
                    Console.Clear();
                    Console.WriteLine("게임을 종료합니다.");
                    Environment.Exit(0);
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
        }

        //상태보기
        static void ShowStatus()
        {
            
            float add = item.add();
            int def = item.def();
            Console.Clear();
            Console.WriteLine("상태보기\n캐릭터의 정보가 표시됩니다.\n");

            Console.WriteLine($"Lv: {player.Level.ToString("D2")}");
            Console.WriteLine($"{player.Name} ({player.Job})");
            Console.WriteLine($"공격력: {player.Attack} (+{add})");
            Console.WriteLine($"방어력: {player.Defense} (+{def})");
            Console.WriteLine($"체력: {player.Health}");
            Console.WriteLine($"골드: {player.Glod}G\n");
            Console.WriteLine("0. 나가기\n");

            Console.WriteLine("원하시는 행동을 입력하세요");
            Console.Write(">>\t");
            string choice = Console.ReadLine();
            if(choice == "0")
            {
                GameStart();
            }
            else
            {
                Console.Clear();
                ShowStatus();
            }
        }

        //인벤토리 보기
        static void ShowInventory()
        {
            Console.Clear();
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
            Console.WriteLine("[아이템 목록]\n");
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine("0. 나가기\n");

            Console.WriteLine("원하시는 행동을 입력하세요");
            Console.Write(">>\t");

            string choice = Console.ReadLine();

            if (choice == "0")
            {
                GameStart();
            }
            else if(choice == "1")
            {
                Console.Clear();
                ShowItem(item);
            }
            else 
            {
                Console.Clear();
            }
        }

        //인벤토리 아이템 보기 - 장착 관리
        static void ShowItem(Item item)
        {
            while (true)
            {
                Console.Clear();
                int atcnt = 0;
                int defcnt = 0;
                int atbasicCnt = 0;
                int defbasicCnt = 0;
                bool isCorrect = false;
                Console.WriteLine("인벤토리 - 장착 관리");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine("[아이템 목록]");

                for (int i = 0; i < item.item.Count; i++)
                {
                    Console.WriteLine(item.item[i]);
                }

                Console.WriteLine();
                Console.WriteLine("0. 나가기\n");

                Console.WriteLine("원하시는 행동을 입력하세요");
                Console.Write(">>\t");

                string choice = Console.ReadLine();

                if (choice == "0")
                {
                    GameStart();
                    break;
                }
                // 아이템 착용 여부 확인 및 중복 착용 방지
                int num;
                if (int.TryParse(choice, out num) && num - 1 < item.item.Count && num > 0)
                {
                    for (int i = 0; i < item.item.Count; i++)
                    {
                        if (item.item[i].Contains("[E]") && item.item[i].Contains("공격력"))
                        {
                            atcnt++;
                            atbasicCnt = i;
                            if (defcnt > 0)
                            {
                                break;
                            }
                        }
                        else if (item.item[i].Contains("[E]") && item.item[i].Contains("방어력"))
                        {
                            defcnt++;
                            defbasicCnt = i;
                            if (atcnt > 0)
                            {
                                break;
                            }
                        }
                    }
                    if (atcnt > 0 || defcnt > 0)
                    {
                        if (defcnt == 0 && item.item[num - 1].Contains("방어력"))
                        {
                            item.change(num);
                            item.WearItem(num, player);
                            isCorrect = true;
                        }
                        else if (atcnt == 0 && item.item[num - 1].Contains("공격력"))
                        {
                            item.change(num);
                            item.WearItem(num, player);
                            isCorrect = true;
                        }
                        else if (item.item[num - 1] == item.item[atbasicCnt])
                        {
                            item.change(num);
                            item.WearItem(num, player);
                            isCorrect = true;
                        }
                        else if (item.item[num - 1] == item.item[defbasicCnt])
                        {
                            item.change(num);
                            item.WearItem(num, player);
                            isCorrect = true;
                        }
                        else
                        {
                            Console.WriteLine("아이템을 장착할 수 없습니다.");
                            Thread.Sleep(1000);
                            isCorrect = true;
                        }
                    }
                    else
                    {
                        item.change(num);
                        item.WearItem(num, player);
                        isCorrect = true;
                    }
                }
                if(!isCorrect)
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
        }

        //상점 보여주기
        static void ShowShop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("상점");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{player.Glod}\n");
                Console.WriteLine("[아이템 목록]");
                for (int i = 0; i < store.store_item.Length; i++)
                {
                    Console.WriteLine(store.store_item[i]);
                }
                Console.WriteLine();
                Console.WriteLine("1. 구매");
                Console.WriteLine("2. 판매");
                Console.WriteLine("0. 나가기");

                Console.WriteLine("원하시는 행동을 입력하세요");
                Console.Write(">>\t");
                string choice = Console.ReadLine();

                if (choice == "0")
                {
                    GameStart();
                    break;
                }
                else if (choice == "1")
                {
                    ShowBuyItem();
                    break;
                }
                else if (choice == "2")
                {
                    ShowSell_item();
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
                    
        }

        //상점 구매하기
        static void ShowBuyItem()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("상점 - 아이템 구매");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{player.Glod}\n");
                Console.WriteLine("[아이템 목록]");
                // 아이템 번호 추가
                for (int i = 0; i < store.store_item.Length; i++)
                {
                    string item = store.store_item[i];
                    Console.WriteLine(item.Replace("- ", $"- {i + 1}. "));
                }
                Console.WriteLine();
                Console.WriteLine("0. 나가기");

                Console.WriteLine("원하시는 아이템의 번호를 입력하세요");
                Console.Write(">>\t");
                string choice = Console.ReadLine();

                int num;
                int.TryParse(choice, out num);

                if (choice == "0")
                {
                    ShowShop();
                    break;
                }
                else if(num > 0 && num <= store.store_item.Length)
                {
                    int restGold = player.Glod; // 골드 차감
                    restGold -= int.Parse(store.itemPirce[num - 1]);
                    if (restGold < 0)
                    {
                        Console.WriteLine("골드가 부족합니다.");
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        store.BuyItem(num, item, player);
                    }
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
        }

        //상점 아이템 판매하기
        static void ShowSell_item()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("상점 - 아이템 판매");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{player.Glod}\n");
                Console.WriteLine("[아이템 목록]");
                // 아이템 번호 추가
                for (int i = 0; i < store.store_Sell_item.Count; i++)
                {
                    string itemNum = store.store_Sell_item[i];
                    Console.WriteLine(itemNum.Replace("- ", $"- {i + 1}. "));
                }
                Console.WriteLine();
                Console.WriteLine("0. 나가기");

                Console.WriteLine("원하시는 아이템의 번호를 입력하세요");
                Console.Write(">>\t");
                string choice = Console.ReadLine();

                int num;
                int.TryParse(choice, out num);

                if (choice == "0")
                {
                    ShowShop();
                    break;
                }
                else if (num > 0 && num <= item.item.Count) // 아이템 문자에서 골드 파악하고 더하기
                {
                    int str = store.store_Sell_item[num - 1].IndexOf("G") - 6;
                    string str2 = store.store_Sell_item[num - 1].Substring(str, 5).Trim();
                    int st = 0;
                    int.TryParse(str2, out st);
                    player.Glod += st;

                    store.Sell_item(num, item);

                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
        }

        //던전 보여주기
        static void ShowDungeon()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("던전\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다..\n");
                Console.WriteLine("1. 쉬운 던전     | 방어력 5 이상 권장\n2. 일반 던전     | 방어력 11 이상 권장\n3. 어려운 던전    | 방어력 17 이상 권장\n0. 나가기");
                Console.WriteLine("원하시는 아이템의 번호를 입력하세요");
                Console.Write(">>\t");
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.Clear();
                    Console.WriteLine("쉬운 던전");
                    Console.WriteLine("던전으로 들어갑니다.");
                    dungeonNum = 0;
                    Thread.Sleep(2000);
                    DungeonStart();
                }
                else if (choice == "2")
                {
                    Console.Clear();
                    Console.WriteLine("일반 던전");
                    Console.WriteLine("던전으로 들어갑니다.");
                    dungeonNum = 1;
                    Thread.Sleep(2000);
                    DungeonStart();
                }
                else if (choice == "3")
                {
                    Console.Clear();
                    Console.WriteLine("어려운 던전");
                    Console.WriteLine("던전으로 들어갑니다.");
                    dungeonNum = 2;
                    Thread.Sleep(2000);
                    DungeonStart();
                }
                else if (choice == "0")
                {
                    GameStart();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Thread.Sleep(2000);
                }
            }
        }

        //던전 시작
        static void DungeonStart()
        {
            Console.Clear();
            Random random = new Random();
            if (player.Defense < dungeonDef[dungeonNum]) // 플레이어 방어가 적정 던전 방어보다 낮을시
            {
                int clearOrNot = random.Next(0, 10);
                if(clearOrNot > 5)
                {
                    Console.WriteLine("던전 클리어 실패!");
                    player.Health -= player.Health / 2;
                    Console.WriteLine($"체력이 {player.Health}이 되었습니다.");
                    Thread.Sleep(2000);
                    GameStart();
                }
                else
                {
                    DungeonClear();
                }
            }
            else
            {
                DungeonClear();
            }
        }

        //던전 클리어
        static void DungeonClear()
        {
            Random random = new Random();
            Console.WriteLine($"던전 클리어!\n축하합니다!!\n{dungeonName[dungeonNum]} 던전을 클리어 하였습니다.\n");
            Thread.Sleep(1000);
            Console.WriteLine("[탐험 결과]\n");
            Thread.Sleep(1000);
            int lostHp = random.Next(20, 36); // 체력 감소량
            lostHp += dungeonDef[dungeonNum] - player.Defense;
            Console.WriteLine($"체력 {player.Health} -> {player.Health - lostHp}");
            Thread.Sleep(1000);
            player.Health -= lostHp;
            float attackGold = random.Next((int)player.Attack, (int)player.Attack * 2 + 1); // 골드 보상
            attackGold /= 100;
            attackGold *= 1000;
            Console.WriteLine($"Gold {player.Glod} G -> {player.Glod + dungeonGlod[dungeonNum] + attackGold}");
            player.Glod = player.Glod + dungeonGlod[dungeonNum] + (int)attackGold;
            Thread.Sleep(2000);
            player.Level++; // 레벨업
            player.Attack += 0.5f;
            player.Defense += 1;
            GameStart();
        }

        //휴식하기
        static void GetRest()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("휴식");
                Console.WriteLine($"500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.Glod} G)\n");
                Console.WriteLine("1. 휴식\n0. 나가기");
                Console.WriteLine("원하시는 아이템의 번호를 입력하세요");
                Console.Write(">>\t");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    player.Glod -= 500;
                    player.Health = 100;
                }
                else if (choice == "0")
                {
                    GameStart();
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Thread.Sleep(1000);
                }
            }
        }

        //게임 저장하기
        static void SaveGame()
        {
            SaveData saveData = new SaveData
            {
                Player = player,
                Item = item,
                StoreItems = store.store_item.ToList()
            };

            string Save = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("saveTextRPG", Save);

            Console.WriteLine("게임이 저장되었습니다!");
            Thread.Sleep(1000);
        }

        //게임 불러오기
        static int LoadGame()
        {
            if (!File.Exists("saveTextRPG"))
            {
                Console.WriteLine("저장된 게임이 없습니다.");
                int isSave = 1;
                Thread.Sleep(1000);
                return isSave;
            }

            string saveFile = File.ReadAllText("saveTextRPG");
            SaveData saveData = JsonSerializer.Deserialize<SaveData>(saveFile);

            player = saveData.Player;
            item = saveData.Item;
            store.store_item = saveData.StoreItems.ToArray();

            Console.WriteLine("게임이 불러와졌습니다!");
            Thread.Sleep(1000);
            return 0;
        }

        //게임 시작 화면
        static void ShowStartView()
        {
            while (true)
            {
                for (int i = 0; i < 38; i++)
                {
                    Console.Write("■");
                }
                Console.WriteLine();
                for (int j = 0; j < 20; j++)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (i == 0 || i == 19)
                        {
                            Console.Write("■");
                        }
                        else
                        {
                            Console.Write("    ");
                        }
                    }
                    Console.WriteLine();
                }
                for (int i = 0; i < 38; i++)
                {
                    Console.Write("■");
                }
                Console.SetCursorPosition(30, 5);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("스파르타 던전");
                Console.SetCursorPosition(29, 9);
                Console.WriteLine("1. 새로운 모험");
                Console.SetCursorPosition(29, 13);
                Console.WriteLine("2. 이어서 모험");
                Console.SetCursorPosition(37, 14);
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.Clear();
                    Console.WriteLine("새로운 모험을 시작합니다.");
                    Thread.Sleep(2000);
                    break;
                }
                else if (choice == "2")
                {
                    Console.Clear();
                    int isSave = LoadGame();
                    if (isSave == 1)
                    {
                        Console.Clear();
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("이어지는 모험을 시작합니다.");
                        isGameSave = 1;
                        Thread.Sleep(2000);
                        break;
                    }
                }
                else
                {
                    Console.Clear();
                }
            }
        }
    }

}
