from collections import defaultdict
from typing import List


def leet_code_49(strs: List[str]) -> List[List[str]]:
    anagram = defaultdict(list)
    result = []
    
    for s in strs:
        sorted_s = tuple(sorted(s))
        anagram[sorted_s].append(s)
        
    for value in anagram.values():
        result.append
        
    return result



if __name__ == '__main__':
    
    print(leet_code_49(["eat", "tea", "tan", "ate", "nat", "bat"]))
    