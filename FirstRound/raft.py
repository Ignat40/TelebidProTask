def raft(capacity, goats, K):
    courses = 0
    i = 0
    n = len(goats)
    
    while i < n:
        courses += 1 
        cur = 0
        
        while i < n and cur + goats[i] <= capacity:
            cur += goats[i]
            i += 1
    
    return courses <= K

def solution(goats, K):    
    goats = sorted(goats, reverse=True)
    
    left = max(goats)
    right = sum(goats)
    
    while left <= right:
        mid = (left + right) // 2
        
        if raft(mid, goats, K):
            ans = mid
            right = mid - 1
        else:
            left = mid + 1
    return ans

goats = [4, 8, 15, 16, 23, 42]
K = 62

result = solution(goats, K)

print(result)
            

# nay dobroto koeto moga da izsmislq :D 