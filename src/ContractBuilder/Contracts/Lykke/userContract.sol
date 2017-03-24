pragma solidity ^0.4.1;

contract QNTB is Owner, QNTBEvent {
    string public name;
    uint8 public decimals;
    uint64 public totalSupply;
    uint64 public totalRemainingToken;
    mapping(address => uint64) public balanceOf;
    enum UserStatus { NonExisted, Existed, Frozen }
    mapping(address => UserStatus) public statusOf;
    
    function issue(address _user, uint64 _amount) onlyOwner public returns (bool);
    function addUser(address _user) onlyOwner public returns (bool);
    function transfer(address _to, uint64 _amount) public returns (bool);
    function frozen(address _user) onlyOwner public returns (bool);
    function unfrozen(address _user) onlyOwner public returns (bool);
}

contract UserContract {
    address _owner;
    address _asset;

    modifier onlyowner { if (msg.sender == _owner) _; }

    function UserContract(address asset) {
        _owner = msg.sender;
        _asset = asset;
    }

    function() payable {
        throw;
    }

    function transferMoney(address recepient, uint value) onlyowner {
        var bank = QNTB(_asset);
        if (!bank.transfer(recepient, value))
            throw;
    }
}